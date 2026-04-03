using Microsoft.AspNetCore.Mvc;
using ReachSystem.Services;
using ReachSystem.Models;
using ReachSystem.Enums;

namespace ReachSystem.Controllers
{
    public class TesteController : Controller
    {
        private readonly AnimalService _service;

        public TesteController(AnimalService service)
        {
            _service = service;
        }

        // 🔥 Criar animal
        public async Task<string> CriarAnimal()
        {
            var animal = new Animal
            {
                Nome = "Bob",
                Especie = "Cachorro",
                Idade = 3,
                Raca = "Vira-lata",
                Porte = "Médio",
                SexoAnimal = Sexo.Macho,
                StatusAnimal = Status.Disponivel,
                DataDeEntrada = DateTime.Now
            };

            await _service.AddAnimalAsync(animal);

            return "Animal criado com sucesso 😏";
        }

        // 🔍 Listar animais
        public async Task<string> Listar()
        {
            var animais = await _service.GetAllAnimalsAsync();

            if (!animais.Any())
                return "Nenhum animal encontrado";

            return string.Join("<br>",
                animais.Select(a => $"ID: {a.AnimalId} - Nome: {a.Nome}"));
        }

        // 🔎 Buscar por ID
        public async Task<string> Buscar(int id)
        {
            var animal = await _service.GetAnimalByIdAsync(id);

            if (animal == null)
                return "Animal não encontrado";

            return $"ID: {animal.AnimalId} - Nome: {animal.Nome} - Espécie: {animal.Especie}";
        }

        // ❌ Deletar
        public async Task<string> Deletar(int id)
        {
            var removido = await _service.DeleteAnimalAsync(id);

            if (!removido)
                return "Animal não encontrado";

            return "Animal deletado com sucesso 😏";
        }
    }
}