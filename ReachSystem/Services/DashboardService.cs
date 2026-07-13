using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Services
{
    public class DashboardService
    {
        private readonly AnimalService _animalService;
        private readonly FichaSaudeService _fichaSaudeService;
        private readonly EventoService _eventoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(
            AnimalService animalService,
            FichaSaudeService fichaSaudeService,
            EventoService eventoService,
            UserManager<ApplicationUser> userManager)
        {
            _animalService = animalService;
            _fichaSaudeService = fichaSaudeService;
            _eventoService = eventoService;
            _userManager = userManager;
        }

        public async Task<int> GetTotalAnimais()
            => await _animalService.CountAsync();

        public async Task<int> GetTotalFichas()
            => await _fichaSaudeService.CountAsync();

        public async Task<int> GetTotalEventos()
            => await _eventoService.CountAsync();

        public async Task<int> GetTotalUsuarios()
            => _userManager.Users.Count();

        public async Task<List<Animal>> GetUltimosAnimaisAsync()
        {
            return await _animalService.GetUltimosAnimaisAsync();
        }
    }
}
