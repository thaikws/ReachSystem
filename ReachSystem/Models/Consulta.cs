namespace ReachSystem.Models
{
    public class Consulta
    {
        public int ConsultaID { get; set; }
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }
        public required string Data { get; set; }
        public required string Descricao { get; set; }
    }
}
