namespace ReachSystem.Models
{
    public class Consulta
    {
        public int ConsultaID { get; set; }
        public int AnimalId { get; set; }
        public animal animal { get; set; }
        public string Data { get; set; }
        public string Descricao { get; set; }
    }
}
