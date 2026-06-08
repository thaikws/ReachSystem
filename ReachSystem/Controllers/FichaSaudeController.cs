using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class FichaSaudeController : Controller
    {

        private readonly FichaSaudeService _fichaSaudeService;

        public FichaSaudeController(FichaSaudeService fichaSaudeService)
        {
            _fichaSaudeService = fichaSaudeService;
        }

        public async Task<IActionResult> Index()
        {
            var fichasSaude = await _fichaSaudeService.GetAllFichasSaudeAsync();
            return View(fichasSaude);
        }

        // GET: FichaSaude/Create
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Create(int animalId)
        {
            var ficha = new FichaSaude { AnimalId = animalId };
            return View(ficha);
        }

        // POST: FichaSaude/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create(FichaSaude fichaSaude)
        {
            if (ModelState.IsValid)
            {
                if (fichaSaude.AnimalId <= 0)
                {
                    return BadRequest("Id do Animal inválido");
                }
                var novaFicha = await _fichaSaudeService.AddFichaSaudeAsync(fichaSaude);
                return RedirectToAction(nameof(Index));
            }
            return View(fichaSaude);
        }

        // GET: FichaSaude/Update/id
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id)
        {
            var fichaSaude = await _fichaSaudeService.GetFichaSaudeByIdAsync(id);
            if (fichaSaude == null)
            {
                return NotFound();
            }
            return View(fichaSaude);
        }

        // POST: FichaSaude/Update/id
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, FichaSaude fichaSaude)
        {
            if (id != fichaSaude.FichaSaudeId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var sucesso = await _fichaSaudeService.UpdateFichaSaudeAsync(fichaSaude);
                if (!sucesso)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fichaSaude);
        }

        // GET: FichaSaude/Delete/id
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int id)
        {
            var fichaSaude = await _fichaSaudeService.GetFichaSaudeByIdAsync(id);
            if (fichaSaude == null)
            {
                return NotFound();
            }
            return View(fichaSaude);
        }

        // POST: FichaSaude/Delete/id
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _fichaSaudeService.DeleteFichaSaudeAsync(id);
            if (!sucesso)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: FichaSaude/Details/id
        public async Task<IActionResult> Details(int id)
        {
            var ficha = await _fichaSaudeService.GetFichaSaudeByIdAsync(id);

            if (ficha == null)
                return NotFound();

            return View(ficha);
        }
    }
}
