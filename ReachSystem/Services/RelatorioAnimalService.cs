using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Enums;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class RelatorioAnimalService
    {
        private readonly ReachSystemDbContext _context;

        public RelatorioAnimalService(ReachSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<Animal>> FiltrarAsync(RelatorioAnimal filtro)
        {
            var animais = await _context.Animais
                .Include(a => a.FichaSaude)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(filtro.Pesquisar))
            {
                animais = animais.Where(a =>
                    a.Nome.Contains(filtro.Pesquisar, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(filtro.Pesquisar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filtro.Especie))
            {
                animais = animais.Where(a =>
                    a.Especie.Equals(filtro.Especie, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (filtro.Sexo.HasValue)
            {
                animais = animais
                    .Where(a => a.SexoAnimal == filtro.Sexo.Value)
                    .ToList();
            }

            if (filtro.Status.HasValue)
            {
                animais = animais
                    .Where(a => a.StatusAnimal == filtro.Status.Value)
                    .ToList();
            }

            if (filtro.DataInicio.HasValue)
            {
                animais = animais
                    .Where(a => a.DataDeEntrada.Date >= filtro.DataInicio.Value.Date)
                    .ToList();
            }

            if (filtro.DataFim.HasValue)
            {
                animais = animais
                    .Where(a => a.DataDeEntrada.Date <= filtro.DataFim.Value.Date)
                    .ToList();
            }

            return animais
                .OrderBy(a => a.Nome)
                .ToList();
        }

        public async Task<List<string>> GetEspeciesAsync()
        {
            return await _context.Animais
                .Select(a => a.Especie)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
        }
    }
}