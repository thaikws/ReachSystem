using Microsoft.Build.Framework;

namespace ReachSystem.Models
{
    public class Evento
    {
        [Required]
        public int EventoId { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public string Local { get; set; } = string.Empty;
        [Required]
        public string Descricao { get; set; } = string.Empty;

        //Relacionamento com outras entidades abaixo pra fazer dps!

        public virtual ICollection<Participacao> Participacoes { get; set; }
            = new List<Participacao>();
    }
}
