namespace buzzaraApi.DTOs
{
    public class PagamentoAnuncioDTO
    {
        public int PagamentoAnuncioID { get; set; }
        public int ServicoID { get; set; }
        public decimal ValorTotal { get; set; }
        public string QrCodeUrl { get; set; } = null!;
        public bool Pago { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
