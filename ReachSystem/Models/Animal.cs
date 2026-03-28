using ReachSystem.Enums;
using System.ComponentModel.DataAnnotations;
namespace ReachSystem.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Raca { get; set; } = string.Empty;
        public string Porte { get; set; } = string.Empty;
        public Sexo SexoAnimal { get; set; }
        public Status StatusAnimal { get; set; }
        public DateTime DataDeEntrada { get; set; }
        public FichaSaude? FichaSaude { get; set; }

        public Animal(string nome, string especie, int idade, string raca, string porte, Sexo sexo, Status status, DateTime dataDeEntrada, FichaSaude fichaSaude)
        {
            Nome = nome;
            Especie = especie;
            Idade = idade;
            Raca = raca;
            Porte = porte;
            SexoAnimal = sexo;
            StatusAnimal = status;
            DataDeEntrada = dataDeEntrada;
            FichaSaude = fichaSaude;
        }
    }
}
