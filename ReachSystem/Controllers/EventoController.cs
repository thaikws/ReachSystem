using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Models;
using ReachSystem.Services;
using ReachSystem.Enums;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        private readonly EventoService _eventoService;
        private readonly ParticipacaoService _participacaoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventoController(
            EventoService eventoService,
            ParticipacaoService participacaoService,
            UserManager<ApplicationUser> userManager)
        {
            _eventoService = eventoService;
            _participacaoService = participacaoService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var eventos = await _eventoService.GetAllEventosAsync();
            return View(eventos);
        }

        // GET Evento/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST Evento/Create
        [HttpPost]
        public async Task<IActionResult> Create(Evento evento)
        {
            if (ModelState.IsValid)
            {
                await _eventoService.AddEventoAsync(evento);
                return RedirectToAction(nameof(Index));
            }

            return View(evento);
        }

        // GET Evento/Update/id
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var evento = await _eventoService.GetEventoByIdAsync(id);

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // POST Evento/Update/id
        [HttpPost]
        public async Task<IActionResult> Update(int id, Evento evento)
        {
            if (id != evento.EventoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var sucesso = await _eventoService.UpdateEventoAsync(evento);

                if (!sucesso)
                    return NotFound();

                TempData["Sucesso"] = "Evento atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(evento);
        }

        // GET Evento/Delete/id
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _eventoService.GetEventoByIdAsync(id);

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // POST Evento/Delete/id
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucesso = await _eventoService.DeleteEventoAsync(id);

            if (!sucesso)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET Evento/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var evento = await _eventoService.GetEventoByIdAsync(id);

            if (evento == null)
                return NotFound();

            ViewBag.Participantes = await _participacaoService.GetParticipantesAsync(id);

            return View(evento);
        }

        // Participar de um evento
        [HttpPost]
        public async Task<IActionResult> Participar(int eventoId)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return Unauthorized();

            var sucesso = await _participacaoService.ParticiparAsync(usuario.Id, eventoId);

            if (!sucesso)
            {
                TempData["Erro"] = "Você já está participando deste evento.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Cancelar participação
        [HttpPost]
        public async Task<IActionResult> CancelarParticipacao(int eventoId)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return Unauthorized();

            await _participacaoService.CancelarParticipacaoAsync(usuario.Id, eventoId);

            return RedirectToAction(nameof(Index));
        }

        // Atualizar status da participação
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> AtualizarStatusParticipacao(int participacaoId, StatusParticipacao status)
        {
            var sucesso = await _participacaoService.AtualizarStatusAsync(participacaoId, status);

            if (!sucesso)
                return NotFound();

            TempData["Sucesso"] = "Status atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}