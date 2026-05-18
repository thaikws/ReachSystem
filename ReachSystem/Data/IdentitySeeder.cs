using Microsoft.AspNetCore.Identity;
using ReachSystem.Models;

namespace ReachSystem.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Funcionario", "Voluntario" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@reach.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "Administrador"
                };

                var result = await userManager.CreateAsync(user, "Admin123@");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            //funcionario
            var funcionarioEmail = "funcionario@reach.com";

            var funcionario = await userManager.FindByEmailAsync(funcionarioEmail);

            if (funcionario == null)
            {
                var user = new ApplicationUser
                {
                    UserName = funcionarioEmail,
                    Email = funcionarioEmail,
                    Nome = "Funcionario Teste"
                };

                var result = await userManager.CreateAsync(user, "Funcionario123@");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Funcionario");
                }
            }
            //voluntario
            var voluntarioEmail = "voluntario@reach.com";

            var voluntario = await userManager.FindByEmailAsync(voluntarioEmail);

            if (voluntario == null)
            {
                var user = new ApplicationUser
                {
                    UserName = voluntarioEmail,
                    Email = voluntarioEmail,
                    Nome = "Voluntario Teste"
                };

                var result = await userManager.CreateAsync(user, "Voluntario123@");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Voluntario");
                }
            }
        }
    }
}
