using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.DTOs;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class ConsultaService
    {
        private readonly ReachSystemDbContext _context;

        public ConsultaService(ReachSystemDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public async Task<List<ConsultaDto>> GetAllConsultasAsync()
        {
            return await _context.Consultas
                .Include(c => c.Animal)
                .Select(c => new ConsultaDto
                {
                    ConsultaID = c.ConsultaID,
                    AnimalId = c.AnimalId,
                    AnimalNome = c.Animal.Nome,
                    AnimalFoto = c.Animal.Foto, // 👈 NOVO
                    Data = c.Data,
                    Descricao = c.Descricao
                })
                .ToListAsync();
        }

        // GET POR ID
        public async Task<ConsultaDto?> GetConsultaByIdAsync(int id)
        {
            return await _context.Consultas
                .Include(c => c.Animal)
                .Where(c => c.ConsultaID == id)
                .Select(c => new ConsultaDto
                {
                    ConsultaID = c.ConsultaID,
                    AnimalId = c.AnimalId,
                    AnimalNome = c.Animal.Nome,
                    AnimalFoto = c.Animal.Foto, // 👈 NOVO
                    Data = c.Data,
                    Descricao = c.Descricao
                })
                .FirstOrDefaultAsync();
        }

        // ADD
        public async Task<Consulta> AddConsultaAsync(Consulta consulta)
        {
            var exists = await _context.Consultas
                .AnyAsync(c =>
                    c.AnimalId == consulta.AnimalId &&
                    c.Data == consulta.Data &&
                    c.Descricao == consulta.Descricao);

            if (exists)
                return consulta;

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return consulta;
        }

        // UPDATE
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

        // DELETE
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