using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class RelatorioEventoService
    {
        private readonly ReachSystemDbContext _context;

        public RelatorioEventoService(ReachSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<Evento>> FiltrarAsync(RelatorioEvento filtro)
        {
            var eventos = await _context.Eventos
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(filtro.Pesquisar))
            {
                eventos = eventos
                    .Where(e =>
                        e.Nome.Contains(
                            filtro.Pesquisar,
                            StringComparison.OrdinalIgnoreCase) ||
                        e.Local.Contains(
                            filtro.Pesquisar,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (filtro.DataInicio.HasValue)
            {
                eventos = eventos
                    .Where(e => e.Data.Date >= filtro.DataInicio.Value.Date)
                    .ToList();
            }

            if (filtro.DataFim.HasValue)
            {
                eventos = eventos
                    .Where(e => e.Data.Date <= filtro.DataFim.Value.Date)
                    .ToList();
            }

            return eventos
                .OrderBy(e => e.Data)
                .ToList();
        }
    }
}