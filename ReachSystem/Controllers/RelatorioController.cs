using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Enums;
using ReachSystem.Models;
using ReachSystem.Services;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class RelatorioController : Controller
    {
        private readonly AnimalService _animalService;

        public RelatorioController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? pesquisar,
            string? especie,
            Sexo? sexo,
            Status? status,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var todosAnimais = await _animalService.GetAllAnimalsAsync();

            // Opções dos filtros
            ViewBag.Especies = todosAnimais
                .Select(a => a.Especie)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .OrderBy(e => e)
                .ToList();

            ViewBag.Pesquisar = pesquisar;
            ViewBag.Especie = especie;
            ViewBag.Sexo = sexo;
            ViewBag.Status = status;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            // Filtro por nome ou raça
            if (!string.IsNullOrWhiteSpace(pesquisar))
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.Nome.Contains(pesquisar, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(pesquisar, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por espécie
            if (!string.IsNullOrWhiteSpace(especie))
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por sexo
            if (sexo.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.SexoAnimal == sexo.Value);
            }

            // Filtro por status
            if (status.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.StatusAnimal == status.Value);
            }

            // Filtro por data inicial
            if (dataInicio.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.DataDeEntrada.Date >= dataInicio.Value.Date);
            }

            // Filtro por data final
            if (dataFim.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.DataDeEntrada.Date <= dataFim.Value.Date);
            }

            return View(todosAnimais.OrderBy(a => a.Nome));
        }
    }
}