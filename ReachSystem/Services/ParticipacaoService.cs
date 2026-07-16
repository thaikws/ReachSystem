using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReachSystem.Data;
using ReachSystem.Enums;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class ParticipacaoService
    {
        private readonly ReachSystemDbContext _context;

        public ParticipacaoService(ReachSystemDbContext context)
        {
            _context = context;
        }

        // Participar de um evento
        public async Task<bool> ParticiparAsync(string usuarioId, int eventoId)
        {
            var existe = await _context.Participacoes
                .AnyAsync(p => p.UsuarioId == usuarioId && p.EventoId == eventoId);

            if (existe)
                return false;

            var participacao = new Participacao
            {
                UsuarioId = usuarioId,
                EventoId = eventoId,
                Status = StatusParticipacao.Confirmado
            };

            _context.Participacoes.Add(participacao);
            await _context.SaveChangesAsync();

            return true;
        }

        // Cancelar participação
        public async Task<bool> CancelarParticipacaoAsync(string usuarioId, int eventoId)
        {
            var participacao = await _context.Participacoes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.EventoId == eventoId);

            if (participacao == null)
                return false;

            _context.Participacoes.Remove(participacao);
            await _context.SaveChangesAsync();

            return true;
        }

        // Mudar Status
        public async Task<bool> AtualizarStatusAsync(int participacaoId, StatusParticipacao status)
        {
            var participacao = await _context.Participacoes
                .FirstOrDefaultAsync(p => p.ParticipacaoId == participacaoId);

            if (participacao == null)
                return false;

            participacao.Status = status;

            await _context.SaveChangesAsync();

            return true;
        }

        // Lista participantes de um evento
        public async Task<IEnumerable<ApplicationUser>> GetParticipantesAsync(int eventoId)
        {
            return await _context.Participacoes
                .Where(p => p.EventoId == eventoId)
                .Select(p => p.Usuario)
                .ToListAsync();
        }

        // Lista eventos de um usuário
        public async Task<IEnumerable<Evento>> GetEventosDoUsuarioAsync(string usuarioId)
        {
            return await _context.Participacoes
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => p.Evento)
                .ToListAsync();
        }

        // Verifica se já participa
        public async Task<bool> UsuarioJaParticipaAsync(string usuarioId, int eventoId)
        {
            return await _context.Participacoes
                .AnyAsync(p => p.UsuarioId == usuarioId && p.EventoId == eventoId);
        }

        // Buscar por ID
        public async Task<Participacao?> GetParticipacaoByIdAsync(int id)
        {
            return await _context.Participacoes
                .Include(p => p.Usuario)
                .Include(p => p.Evento)
                .FirstOrDefaultAsync(p => p.ParticipacaoId == id);
        }
    }
}