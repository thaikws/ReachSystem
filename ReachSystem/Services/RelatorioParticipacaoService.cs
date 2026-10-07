using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class RelatorioParticipacaoService
    {
        private readonly ReachSystemDbContext _context;

        public RelatorioParticipacaoService(ReachSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<Participacao>> FiltrarAsync(RelatorioParticipacao filtro)
        {
            var participacoes = await _context.Participacoes
                .Include(p => p.Usuario)
                .Include(p => p.Evento)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(filtro.Pesquisar))
            {
                participacoes = participacoes
                    .Where(p =>
                        p.Usuario.Nome.Contains(
                            filtro.Pesquisar,
                            StringComparison.OrdinalIgnoreCase) ||
                        p.Usuario.Email.Contains(
                            filtro.Pesquisar,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filtro.Evento))
            {
                participacoes = participacoes
                    .Where(p =>
                        p.Evento.Nome.Equals(
                            filtro.Evento,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (filtro.Status.HasValue)
            {
                participacoes = participacoes
                    .Where(p => p.Status == filtro.Status.Value)
                    .ToList();
            }

            if (filtro.DataInicio.HasValue)
            {
                participacoes = participacoes
                    .Where(p =>
                        p.Evento.Data.Date >= filtro.DataInicio.Value.Date)
                    .ToList();
            }

            if (filtro.DataFim.HasValue)
            {
                participacoes = participacoes
                    .Where(p =>
                        p.Evento.Data.Date <= filtro.DataFim.Value.Date)
                    .ToList();
            }

            return participacoes
                .OrderBy(p => p.Evento.Data)
                .ThenBy(p => p.Usuario.Nome)
                .ToList();
        }

        public async Task<List<string>> GetEventosAsync()
        {
            return await _context.Participacoes
                .Include(p => p.Evento)
                .Select(p => p.Evento.Nome)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
        }
    }
}