using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class EventoService
    {
        // Injeção de dependência do DbContext
        private readonly ReachSystemDbContext _context;
        public EventoService(ReachSystemDbContext context)
        {
            _context = context;
        }

        //Método Get
        public async Task<IEnumerable<Evento>> GetAllEventosAsync()
        {
            return await _context.Eventos.ToListAsync();
        }

        //Método Get por Id
        public async Task<Evento?> GetEventoByIdAsync(int id)
        {
            return await _context.Eventos.FirstOrDefaultAsync(e => e.EventoId == id);
        }

        //Método Add
        public async Task<Evento> AddEventoAsync(Evento evento)
        {
            if (string.IsNullOrWhiteSpace(evento.Nome) || string.IsNullOrWhiteSpace(evento.Local) || evento.Data == default)
            {
                throw new ArgumentException("Dados inválidos");
            }
            else
            {
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
            }
            return evento;
        }

        //Método Update
        public async Task<bool> UpdateEventoAsync(Evento evento)
        {
            var existente = await _context.Eventos.FindAsync(evento.EventoId);

            if (existente == null)
                return false;
            else
            {
                existente.Nome = evento.Nome;
                existente.Local = evento.Local;
                existente.Data = evento.Data;
                existente.Descricao = evento.Descricao;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        //Método Delete
        public async Task<bool> DeleteEventoAsync(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);

            if (evento == null)
                return false;
            else
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
