using ReachSystem.Models;
using ReachSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Models.ViewModels;

namespace ReachSystem.Services
{
    public class UsuarioService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UsuarioViewModel>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var usuarios = new List<UsuarioViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usuarios.Add(new UsuarioViewModel
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "Sem Role"
                });
            }

            return usuarios;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }


        public async Task<IdentityResult> CreateUserAsync(string nome, string email, string senha, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new Exception("Role inválida.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Nome = nome
            };

            var result = await _userManager.CreateAsync(user, senha);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(
            string id,
            string nome,
            string email)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            user.Nome = nome;
            user.Email = email;
            user.UserName = email;

            return await _userManager.UpdateAsync(user);
        }

        public async Task ChangeRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, newRole);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            await _userManager.DeleteAsync(user);
        }
    }
}
