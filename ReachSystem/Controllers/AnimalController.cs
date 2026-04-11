using Microsoft.AspNetCore.Mvc;
using ReachSystem.Services;
using ReachSystem.Models;

namespace ReachSystem.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;

        public AnimalController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        public async Task<IActionResult> Index(string pesquisarString)
        {
            var animais = await _animalService.GetAllAnimalsAsync();
            if (!string.IsNullOrEmpty(pesquisarString))
            {
                animais = animais.Where(a =>
                    a.Nome.Contains(pesquisarString, StringComparison.OrdinalIgnoreCase) ||
                    a.Especie.Contains(pesquisarString, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(pesquisarString, StringComparison.OrdinalIgnoreCase));
            }
            return View(animais);
        }

        // GET: Animal/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Animal/Create
        [HttpPost]
        public async Task<IActionResult> Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                await _animalService.AddAnimalAsync(animal);
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        // GET: Animal/Update/id
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // POST: Animal/Update/id
        [HttpPost]
        public async Task<IActionResult> Update(int id, Animal animal)
        {
            if (id != animal.AnimalId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var sucesso = await _animalService.UpdateAnimalAsync(animal);
                if (!sucesso)
                {
                    return NotFound();
                }
                TempData["Sucesso"] = "Animal atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        // GET: Animal/Delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // POST: Animal/Delete/id
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _animalService.DeleteAnimalAsync(id);
            if (!sucesso)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Animal/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }
    }
}
