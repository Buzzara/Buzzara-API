namespace buzzaraApi.DTOs
{
    public class ServicoCacheDTO
    {
        public string FormaPagamento { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Valor { get; set; }
    }
}
