using Microsoft.AspNetCore.Identity;
using Sispat.Domain.Entities;

namespace Sispat.Infrastructure.Identity
{
    // Classe para popular o banco com dados iniciais
    public class IdentityDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Método principal
        public async Task SeedRolesAndAdminAsync()
        {
            // 1. Criar os Níveis (Roles) se não existirem
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // 2. Criar um Usuário Admin padrão se não existir
            var adminUser = await _userManager.FindByEmailAsync("admin@sispat.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = "Administrador",
                    Email = "admin@sispat.com",
                    UserName = "admin@sispat.com"
                };

                // Cria o usuário com a senha
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // 3. Adiciona o usuário ao nível "Admin"
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}