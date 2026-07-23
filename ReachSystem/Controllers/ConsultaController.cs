using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReachSystem.Models;
using ReachSystem.Services;
using ReachSystem.DTOs;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class ConsultaController : Controller
    {
        private readonly ConsultaService _service;
        private readonly AnimalService _animalService;

        public ConsultaController(ConsultaService service, AnimalService animalService)
        {
            _service = service;
            _animalService = animalService;
        }

        // LISTAR
        public async Task<IActionResult> Index()
        {
            var consultas = await _service.GetAllConsultasAsync();
            return View(consultas);
        }

        // DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var consulta = await _service.GetConsultaByIdAsync(id);

            if (consulta == null)
                return NotFound();

            return View(consulta);
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var animais = await _animalService.GetAllAnimalsAsync();

            ViewBag.Animais = animais; 
            return View();
        }

        //POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(ConsultaDto consulta)
        {
            if (!ModelState.IsValid)
            {
                var animais = await _animalService.GetAllAnimalsAsync();
                ViewBag.Animais = animais;

                return View(consulta);
            }

            var entity = new Consulta
            {
                AnimalId = consulta.AnimalId,
                Data = DateTime.SpecifyKind(consulta.Data, DateTimeKind.Local),
                Descricao = consulta.Descricao
            };

            await _service.AddConsultaAsync(entity);

            return RedirectToAction(nameof(Index));
        }

        // GET: Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var consulta = await _service.GetConsultaByIdAsync(id);

            if (consulta == null)
                return NotFound();

            return View(consulta);
        }

        // POST: Update
        [HttpPost]
        public async Task<IActionResult> Update(int id, Consulta consulta)
        {
            if (id != consulta.ConsultaID)
                return NotFound();

            if (ModelState.IsValid)
            {
                var sucesso = await _service.UpdateConsultaAsync(consulta);

                if (!sucesso)
                    return NotFound();

                TempData["Sucesso"] = "Consulta atualizada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(consulta);
        }

        // GET: Delete
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await _service.GetConsultaByIdAsync(id);

            if (consulta == null)
                return NotFound();

            return View(consulta);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _service.DeleteConsultaAsync(id);

            if (!sucesso)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}