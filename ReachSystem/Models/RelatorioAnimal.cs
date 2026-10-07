using ReachSystem.Enums;

namespace ReachSystem.Models
{
    public class RelatorioAnimal
    {
        public string? Pesquisar { get; set; }
        public string? Especie { get; set; }
        public Sexo? Sexo { get; set; }
        public Status? Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}