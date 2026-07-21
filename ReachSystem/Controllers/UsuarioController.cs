using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReachSystem.Services;
using System.Security.Claims;

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
            var result = await _usuarioService.CreateUserAsync(nome, email, senha, role);

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

        //get update
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var usuario = await _usuarioService.GetUserByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.Role = await _usuarioService.GetUserRoleAsync(id);
            return View(usuario);
        }

        //post update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, string nome, string email, string role, string novaSenha)
        {
            var result = await _usuarioService.UpdateUserAsync(id, nome, email);

            if (result.Succeeded)
            {
                await _usuarioService.ChangeRoleAsync(id, role);

                if (!string.IsNullOrWhiteSpace(novaSenha))
                {
                    var passwordResult = await _usuarioService.ChangePasswordAsync(id, novaSenha);


                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }


                        var user = await _usuarioService.GetUserByIdAsync(id);

                        return View(user);
                    }
                    TempData["Sucesso"] = "Senha alterada com sucesso!";
                }

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            var usuario = await _usuarioService.GetUserByIdAsync(id);
            return View(usuario);
        }

        //get delete
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _usuarioService.GetUserByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //post delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedUserId == id)
            {
                TempData["Erro"] = "Você não pode excluir sua própria conta.";
                return RedirectToAction(nameof(Index));
            }

            await _usuarioService.DeleteUserAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
