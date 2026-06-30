using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalAnimais = await _dashboardService.GetTotalAnimais();
            ViewBag.TotalEventos = await _dashboardService.GetTotalEventos();
            ViewBag.TotalFichas = await _dashboardService.GetTotalFichas();
            ViewBag.TotalUsuarios = await _dashboardService.GetTotalUsuarios();

            return View();
        }
    }
}
