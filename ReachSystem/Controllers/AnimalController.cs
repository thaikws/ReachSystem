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
            if (!ModelState.IsValid)
                return View(animal);

            if (fotoArquivo != null)
            {
                string nomeArquivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(fotoArquivo.FileName);

                string pasta = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "animais"
                );

                Directory.CreateDirectory(pasta);

                string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await fotoArquivo.CopyToAsync(stream);
                }

                animal.Foto = "/images/animais/" + nomeArquivo;
            }

            await _animalService.AddAnimalAsync(animal);

            return RedirectToAction(nameof(Index));
        }

        // GET: Animal/Update/id
        [HttpGet("Animal/Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);

            if (animal == null)
                return NotFound();

            return View(animal);
        }

        // POST: Animal/Update/id
        [HttpPost("Animal/Update/{id}")]
        public async Task<IActionResult> Update(int id, Animal animal, IFormFile? fotoArquivo, string? imagemCortada)
        {
            if (id != animal.AnimalId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(animal);

            var animalExistente = await _animalService.GetAnimalByIdAsync(id);

            if (animalExistente == null)
                return NotFound();

            string? fotoAtual = animalExistente.Foto;

            // =========================
            // 1. ATUALIZA CAMPOS SEMPRE
            // =========================
            animalExistente.Nome = animal.Nome;
            animalExistente.Especie = animal.Especie;
            animalExistente.Raca = animal.Raca;
            animalExistente.Idade = animal.Idade;
            animalExistente.Porte = animal.Porte;
            animalExistente.SexoAnimal = animal.SexoAnimal;
            animalExistente.StatusAnimal = animal.StatusAnimal;
            animalExistente.DataDeEntrada = animal.DataDeEntrada;

            // =========================
            // 2. IMAGEM CORTADA (PRIORIDADE)
            // =========================
            if (!string.IsNullOrEmpty(imagemCortada) && imagemCortada.Contains(","))
            {
                string base64 = imagemCortada.Split(',')[1];
                byte[] bytes = Convert.FromBase64String(base64);

                string nomeArquivo = Guid.NewGuid() + ".jpg";

                string pasta = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "animais");

                Directory.CreateDirectory(pasta);

                string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

                await System.IO.File.WriteAllBytesAsync(caminhoCompleto, bytes);

                animalExistente.Foto = "/images/animais/" + nomeArquivo;
            }
            // =========================
            // 3. IMAGEM NORMAL
            // =========================
            else if (fotoArquivo != null)
            {
                if (!string.IsNullOrEmpty(fotoAtual))
                {
                    string caminhoAntigo = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        fotoAtual.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(caminhoAntigo))
                        System.IO.File.Delete(caminhoAntigo);
                }

                string nomeArquivo =
                    Guid.NewGuid() + Path.GetExtension(fotoArquivo.FileName);

                string pasta = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "animais");

                Directory.CreateDirectory(pasta);

                string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await fotoArquivo.CopyToAsync(stream);
                }

                animalExistente.Foto = "/images/animais/" + nomeArquivo;
            }

            // =========================
            // 4. SALVA UMA VEZ
            // =========================
            await _animalService.UpdateAnimalAsync(animalExistente);

            TempData["Sucesso"] = "Animal atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
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
