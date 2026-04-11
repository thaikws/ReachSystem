using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    public class ConsultaController : Controller
    {
        private readonly ConsultaService _service;

        public ConsultaController(ConsultaService service)
        {
            _service = service;
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(Consulta consulta)
        {
            if (ModelState.IsValid)
            {
                await _service.AddConsultaAsync(consulta);
                return RedirectToAction(nameof(Index));
            }
            return View(consulta);
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
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await _service.GetConsultaByIdAsync(id);

            if (consulta == null)
                return NotFound();

            return View(consulta);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _service.DeleteConsultaAsync(id);

            if (!sucesso)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}