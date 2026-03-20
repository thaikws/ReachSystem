using System.ComponentModel.DataAnnotations;

namespace ReachSystem.Models
{
    public class FichaSaude 
    {
        public int FichaSaudeId { get; set; }
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Vacinas { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string DoencasPreExistentes { get; set; } = string.Empty;
        public string Medicamentos { get; set; } = string.Empty;
    }
}
