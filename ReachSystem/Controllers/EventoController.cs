using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        private readonly EventoService _eventoService;
        public EventoController(EventoService eventoService)
        {
            _eventoService = eventoService;
        }

        public async Task<IActionResult> Index()
        {
            var eventos = await _eventoService.GetAllEventosAsync();
            return View(eventos);
        }

        //Get Evento/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post Evento/Create
        [HttpPost]
        public async Task<IActionResult> Create(Evento evento)
        {
            if (ModelState.IsValid)
            {
                var eventos = await _eventoService.AddEventoAsync(evento);
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        //Get Evento/Update/id
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var eventos = await _eventoService.GetEventoByIdAsync(id);
            if(eventos == null)
            {
                return NotFound();
            }
            else
            {
                return View(eventos);
            }
        }

        //Post Evento/Update/id
        [HttpPost]
        public async Task<IActionResult> Update(int id, Evento evento)
        {
            if(id != evento.EventoId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var sucesso = await _eventoService.UpdateEventoAsync(evento);
                if(!sucesso)
                {
                    return NotFound();
                }
                TempData["Sucesso"] = "Evento atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        //Get Evento/Delete/id
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int id)
        {
            var eventos = await _eventoService.GetEventoByIdAsync(id);
            if(eventos == null)
            {
                return NotFound();
            }
            return View(eventos);
        }

        //Post Evento/Delete/id
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _eventoService.DeleteEventoAsync(id);
            if (!sucesso)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
