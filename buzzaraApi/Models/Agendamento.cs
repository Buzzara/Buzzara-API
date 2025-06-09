namespace buzzaraApi.Models
{
    public class Agendamento
    {
        public int AgendamentoID { get; set; }
        public int ClienteID { get; set; }
        public required Usuario Cliente { get; set; }
        public int PerfilAcompanhanteID { get; set; }
        public required PerfilAcompanhante PerfilAcompanhante { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = "Pendente";
    }
}
