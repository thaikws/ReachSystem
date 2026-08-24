using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class PerfilService
    {
        private readonly ReachSystemDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PerfilService(
            ReachSystemDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetPerfilAsync(string usuarioId)
        {
            return await _context.Users
                .Include(u => u.Participacoes)
                    .ThenInclude(p => p.Evento)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
        }

        public async Task<string> GetRoleAsync(ApplicationUser usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);

            return roles.FirstOrDefault() ?? "Sem função";
        }
    }
}