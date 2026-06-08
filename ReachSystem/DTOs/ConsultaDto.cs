namespace ReachSystem.DTOs
{
    public class ConsultaDto
    {
        public int ConsultaID { get; set; }
        public int AnimalId { get; set; }
        public string AnimalNome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
