using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnimalController(AnimalService animalService, IWebHostEnvironment webHostEnvironment)
        {
            _animalService = animalService;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(Animal animal, IFormFile? fotoArquivo)
        {
            if (ModelState.IsValid)
            {
                if (fotoArquivo != null)
                {
                    string nomeArquivo =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(fotoArquivo.FileName);

                    string pasta = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "animais");

                    Directory.CreateDirectory(pasta);

                    string caminhoCompleto =
                        Path.Combine(pasta, nomeArquivo);

                    using (var stream = new FileStream(
                        caminhoCompleto,
                        FileMode.Create))
                    {
                        await fotoArquivo.CopyToAsync(stream);
                    }

                    animal.Foto = "/images/animais/" + nomeArquivo;
                }

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
        [Authorize(Roles = "Admin,Funcionario")]
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
        [Authorize(Roles = "Admin,Funcionario")]
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
