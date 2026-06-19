using ReachSystem.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ReachSystem.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public string Especie { get; set; } = string.Empty;
        [Required]
        public int Idade { get; set; }
        [Required]
        public string Raca { get; set; } = string.Empty;
        [Required]
        public string Porte { get; set; } = string.Empty;
        [Required]
        public Sexo SexoAnimal { get; set; }
        [Required]
        public Status StatusAnimal { get; set; }
        [Required]
        public DateTime DataDeEntrada { get; set; }
        public string? Foto { get; set; }
        public FichaSaude? FichaSaude { get; set; }
        [ValidateNever]
        public List<Consulta> Consultas { get; set; } = new();

        public Animal() { }
    }
}
