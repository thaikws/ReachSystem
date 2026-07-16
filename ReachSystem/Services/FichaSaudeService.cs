using Microsoft.EntityFrameworkCore;
using ReachSystem.Data;
using ReachSystem.Models;

namespace ReachSystem.Services
{
    public class FichaSaudeService
    {
        private readonly ReachSystemDbContext _context;

        public FichaSaudeService(ReachSystemDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public async Task<IEnumerable<FichaSaude>> GetAllFichasSaudeAsync()
        {
            return await _context.FichasSaude
                .Include(f => f.Animal)
                .ToListAsync();
        }

        // GET POR ID
        public async Task<FichaSaude?> GetFichaSaudeByIdAsync(int id)
        {
            return await _context.FichasSaude
                .Include(f => f.Animal)
                .FirstOrDefaultAsync(f => f.FichaSaudeId == id);
        }

        // ADD
        public async Task<FichaSaude> AddFichaSaudeAsync(FichaSaude fichaSaude)
        {
            if (string.IsNullOrWhiteSpace(fichaSaude.Descricao))
                throw new ArgumentException("Descrição é obrigatória!");

            if (fichaSaude.AnimalId <= 0)
                throw new ArgumentException("Id do Animal inválido");

            _context.FichasSaude.Add(fichaSaude);
            await _context.SaveChangesAsync();

            return fichaSaude;
        }

        // UPDATE
        public async Task<bool> UpdateFichaSaudeAsync(FichaSaude fichaSaude)
        {
            var existente = await _context.FichasSaude
                .FindAsync(fichaSaude.FichaSaudeId);

            if (existente == null)
                return false;

            existente.Descricao = fichaSaude.Descricao;
            existente.Vacinas = fichaSaude.Vacinas;
            existente.Alergias = fichaSaude.Alergias;
            existente.DoencasPreExistentes = fichaSaude.DoencasPreExistentes;
            existente.Medicamentos = fichaSaude.Medicamentos;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteFichaSaudeAsync(int id)
        {
            var ficha = await _context.FichasSaude.FindAsync(id);

            if (ficha == null)
                return false;

            _context.FichasSaude.Remove(ficha);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> CountAsync()
        {
            return await _context.FichasSaude.CountAsync();
        }
    }
}