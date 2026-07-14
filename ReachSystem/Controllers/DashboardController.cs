using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(DashboardService dashboardService, UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalAnimais = await _dashboardService.GetTotalAnimais();
            ViewBag.TotalEventos = await _dashboardService.GetTotalEventos();
            ViewBag.TotalFichas = await _dashboardService.GetTotalFichas();
            ViewBag.TotalUsuarios = await _dashboardService.GetTotalUsuarios();
            ViewBag.UltimosAnimais = await _dashboardService.GetUltimosAnimaisAsync();

            var usuario = await _userManager.GetUserAsync(User);
            ViewBag.NomeUsuario = usuario?.Nome;

            return View();
        }
    }
}
