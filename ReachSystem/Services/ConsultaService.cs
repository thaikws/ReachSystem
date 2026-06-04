using ReachSystem.Models;
using ReachSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ReachSystem.Services
{
    public class ConsultaService
    {
        private readonly ReachSystemDbContext _context;

        public ConsultaService(ReachSystemDbContext context)
        {
            _context = context;
        }

        // Método Get para retornar as consultas com o Animal incluído
        public async Task<IEnumerable<Consulta>> GetAllConsultasAsync()
        {
            return await _context.Consultas
                .Include(c => c.Animal)
                .ToListAsync();
        }

        // Método Get por Id
        public async Task<Consulta?> GetConsultaByIdAsync(int id)
        {
            return await _context.Consultas
                .Include(c => c.Animal)
                .FirstOrDefaultAsync(c => c.ConsultaID == id);
        }

        // Add
        public async Task<Consulta> AddConsultaAsync(Consulta consulta)
        {
            if (consulta.AnimalId <= 0 || string.IsNullOrWhiteSpace(consulta.Descricao))
            {
                throw new ArgumentException("Dados inválidos");
            }

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return consulta;
        }

        // Update
        public async Task<bool> UpdateConsultaAsync(Consulta consulta)
        {
            var existente = await _context.Consultas.FindAsync(consulta.ConsultaID);

            if (existente == null)
                return false;

            existente.AnimalId = consulta.AnimalId;
            existente.Data = consulta.Data;
            existente.Descricao = consulta.Descricao;

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete
        public async Task<bool> DeleteConsultaAsync(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);

            if (consulta == null)
                return false;

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}