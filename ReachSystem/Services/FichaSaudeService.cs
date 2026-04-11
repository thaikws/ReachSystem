using ReachSystem.Models;
using ReachSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ReachSystem.Services
{
    public class FichaSaudeService
    {
        // Injeção de dependência do DbContext
        private readonly ReachSystemDbContext _context;
        public FichaSaudeService(ReachSystemDbContext context)
        {
            _context = context;
        }

        //Método Get
        public async Task<IEnumerable<FichaSaude>> GetAllFichasSaudeAsync()
        {
            return await _context.FichasSaude.Include(a => a.Animal).ToListAsync();
        }

        //Método Get por Id
        public async Task<FichaSaude?> GetFichaSaudeByIdAsync(int id)
        {
            return await _context.FichasSaude.Include(a => a.Animal).FirstOrDefaultAsync(a => a.FichaSaudeId == id);
        }

        //Método Add
        public async Task<FichaSaude> AddFichaSaudeAsync(FichaSaude fichaSaude)
        {
            if (string.IsNullOrWhiteSpace(fichaSaude.Descricao))
            {
                throw new ArgumentException("Descrição é obrigatória!");
            }
            else
            if (fichaSaude.AnimalId <= 0)
            {
                throw new ArgumentException("Id do Animal inválido");
            }
            else
            {
                _context.FichasSaude.Add(fichaSaude);
                await _context.SaveChangesAsync();
            }
            return fichaSaude;
        }

        //Método Update
        public async Task<bool> UpdateFichaSaudeAsync(FichaSaude fichaSaude)
        {
            var existente = await _context.FichasSaude.FindAsync(fichaSaude.FichaSaudeId);

            if (existente == null)
                return false;
            else
            {
                existente.Descricao = fichaSaude.Descricao;
                existente.Vacinas = fichaSaude.Vacinas;
                existente.Alergias = fichaSaude.Alergias;
                existente.DoencasPreExistentes = fichaSaude.DoencasPreExistentes;
                existente.Medicamentos = fichaSaude.Medicamentos;

            }
            await _context.SaveChangesAsync();
            return true;
        }

        //Método Delete
        public async Task<bool> DeleteFichaSaudeAsync(int id)
        {
            var ficha = await _context.FichasSaude.FindAsync(id);

            if (ficha == null)
                return false;
            else
            {
                _context.FichasSaude.Remove(ficha);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
