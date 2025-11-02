using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq; // Necessário para .Select() e .Concat()
using System.Threading.Tasks;

namespace Sispat.API.Controllers
{
    // Este controlador herda de BaseApiController (rota 'api/[controller]')
    public class AuthController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        // Injeção de dependência dos serviços do Identity e Configuração
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint para registrar um novo usuário.
        /// Restrito apenas a usuários com o nível "Admin".
        /// </summary>
        [Authorize(Roles = "Admin")] // <-- Protege a rota
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Este email já está cadastrado." });
            }

            var newUser = new ApplicationUser
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email // UserName é obrigatório
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = errors });
            }

            // --- Regra de Negócio ---
            // Adiciona o novo usuário ao nível "User" por padrão
            await _userManager.AddToRoleAsync(newUser, "User");

            // Retorna sucesso (sem logar automaticamente o novo usuário)
            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Usuário criado com sucesso!"
            });
        }

        /// <summary>
        /// Endpoint público para login de usuários.
        /// </summary>
        [AllowAnonymous] // Permite acesso público (mesmo sendo o padrão)
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Credenciais inválidas." });
            }

            // Verifica a senha
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Credenciais inválidas." });
            }

            // Gera o token (que agora inclui o Nível de Acesso)
            var (token, expiration) = await GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login bem-sucedido.",
                Token = token,
                TokenExpiration = expiration,
                FullName = user.FullName,
                Email = user.Email
            });
        }

        /// <summary>
        /// Método auxiliar privado para gerar o Token JWT.
        /// Agora inclui o(s) Nível(is) de Acesso (Roles) do usuário nos claims.
        /// </summary>
        private async Task<(string token, DateTime expiration)> GenerateJwtToken(ApplicationUser user)
        {
            // 1. Busca os níveis (roles) do usuário
            var roles = await _userManager.GetRolesAsync(user);

            // 2. Define os claims básicos
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id), // Claim customizada com o ID do usuário
                new Claim(ClaimTypes.Name, user.FullName)
            };

            // 3. Define os claims de Nível de Acesso (Role)
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            // 4. Combina todos os claims
            var allClaims = claims.Concat(roleClaims);

            // 5. Pega as configurações do appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(8); // Token válido por 8 horas

            // 6. Cria o token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: allClaims, // Usa os claims combinados
                expires: expiration,
                signingCredentials: creds
            );

            // 7. Retorna o token serializado e sua expiração
            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}