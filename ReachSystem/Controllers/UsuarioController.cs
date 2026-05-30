using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        //Get index
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllUsers();
            return View(usuarios);
        }

        //get create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nome, string email, string senha, string role)
        {
            var result = await _usuarioService.CreateUserAsync(
                nome,
                email,
                senha,
                role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}
