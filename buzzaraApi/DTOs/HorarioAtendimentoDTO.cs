namespace buzzaraApi.DTOs
{
    public class HorarioAtendimentoDTO
    {
        public string? DiaSemana { get; set; }
        public bool Atende { get; set; }
        public string? HorarioInicio { get; set; }  // <- isso deve ser string
        public string? HorarioFim { get; set; }     // <- isso também
        public bool VinteQuatroHoras { get; set; }
    }

}
