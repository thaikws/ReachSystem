using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReachSystem.Services;
using System.Security.Claims;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly PerfilService _perfilService;

        public  PerfilController(PerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(usuarioId == null)
            {
                return Unauthorized();
            }

            var usuario = await _perfilService.GetPerfilAsync(usuarioId);
            if(usuario == null)
            {
                return NotFound();
            }
            ViewBag.Role = await _perfilService.GetRoleAsync(usuario);
            return View(usuario);
        }
    }
}
