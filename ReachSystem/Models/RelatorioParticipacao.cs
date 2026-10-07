using ReachSystem.Enums;

namespace ReachSystem.Models
{
    public class RelatorioParticipacao
    {
        public string? Pesquisar { get; set; }

        public string? Evento { get; set; }

        public StatusParticipacao? Status { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }
    }
}