using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sispat.Application.Interfaces;
using Sispat.Application.Mappings;
using Sispat.Application.Services;
using Sispat.Application.Validators;
using Sispat.Domain.Entities;
using Sispat.Domain.Interfaces;
using Sispat.Infrastructure.Persitence;
using Sispat.Infrastructure.Repositories;
using System.Text;
using Sispat.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddScoped<IdentityDataSeeder>();

// 1. Configurar CORS (Para o Angular poder acessar a API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:52674") // Modificar se necessário
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 2. Configurar EF Core e SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

// 3. Configurar .NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 4. Configurar Autenticação JWT (Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
    };
});

// 5. Configurar Injeção de Dependência (Nossos Serviços)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAssetService, AssetService>();
// (Adicione outros serviços aqui conforme crescer)

// 6. Configurar AutoMapper
// Escaneia o assembly da Application procurando por classes que herdam de 'Profile'
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 7. Configurar FluentValidation
// Registra validadores do assembly da Application
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(CreateAssetValidator).Assembly);

// 8. Adicionar Serviços de API Padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 9. Configurar Swagger para usar Autenticação JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sispat API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' [espaço] e seu token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// --- Construção do App (Pipeline de HTTP) ---

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Aplica migrações pendentes e cria o banco se não existir
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    // Popula o banco com dados iniciais (Níveis e Usuário Admin)
    var seeder = services.GetRequiredService<IdentityDataSeeder>();
    await seeder.SeedRolesAndAdminAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adiciona o middleware de CORS (DEVE VIR ANTES de UseAuthorization)
app.UseCors("AllowAngularApp");

// Adiciona os middlewares de Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();