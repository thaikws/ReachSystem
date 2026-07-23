

using ReachSystem.Enums;

namespace ReachSystem.Models
{
    public class Participacao
    {
        public int ParticipacaoId { get; set; }

        public string UsuarioId { get; set; }
        public int EventoId { get; set; }
        public StatusParticipacao Status { get; set; }

        public virtual ApplicationUser Usuario { get; set; }
        public virtual Evento Evento { get; set; }
    }
}
