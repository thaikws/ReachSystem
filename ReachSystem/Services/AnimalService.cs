using ReachSystem.Models;
using ReachSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ReachSystem.Services
{
    public class AnimalService
    {
        // Injeção de dependência do DbContext
        private readonly ReachSystemDbContext _context;
        public AnimalService(ReachSystemDbContext context)
        {
            _context = context;
        }

        //Método Get
        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await _context.Animais.Include(a => a.FichaSaude).ToListAsync();
        }

        //Método Get por Id
        public async Task<Animal?> GetAnimalByIdAsync(int id)
        {
            return await _context.Animais.Include(a => a.FichaSaude).FirstOrDefaultAsync(a => a.AnimalId == id);
        }

        //Método Add
        public async Task<Animal> AddAnimalAsync(Animal animal)
        {
            if (string.IsNullOrWhiteSpace(animal.Nome) || string.IsNullOrWhiteSpace(animal.Especie))
            {
                throw new ArgumentException("Dados inválidos");
            }
            else
            {
                _context.Animais.Add(animal);
                await _context.SaveChangesAsync();
            }
            return animal;
        }

        //Método Update
        public async Task<bool> UpdateAnimalAsync(Animal animal)
        {
            var existente = await _context.Animais.FindAsync(animal.AnimalId);

            if (existente == null)
                return false;
            else
            {
                existente.Nome = animal.Nome;
                existente.Especie = animal.Especie;
                existente.Idade = animal.Idade;
                existente.Raca = animal.Raca;
                existente.Porte = animal.Porte;
                existente.SexoAnimal = animal.SexoAnimal;
                existente.StatusAnimal = animal.StatusAnimal;
                existente.DataDeEntrada = animal.DataDeEntrada;

            }
            await _context.SaveChangesAsync();
            return true;
        }

        //Método Delete
        public async Task<bool> DeleteAnimalAsync(int id)
        {
            var animal = await _context.Animais.FindAsync(id);

            if (animal == null)
                return false;
            else
            {
                _context.Animais.Remove(animal);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
