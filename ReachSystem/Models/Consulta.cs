using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ReachSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ReachSystem.Models
{
    public class Consulta
    {
        public int ConsultaID { get; set; }

        [Required(ErrorMessage = "Selecione um animal")]
        public int AnimalId { get; set; }

        [ValidateNever]
        public Animal? Animal { get; set; }

        [Required(ErrorMessage = "Data obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Descrição obrigatória")]
        public string Descricao { get; set; } = string.Empty;
    }
}
