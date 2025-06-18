namespace buzzaraApi.Models
{
    public class ServicoCache
    {
        public int Id { get; set; }

        // FK para o Serviço (Anúncio)
        public int ServicoId { get; set; }
        public Servico Servico { get; set; } = null!;

        public string FormaPagamento { get; set; } = null!;   // Exemplo: "Cartão de crédito", "Dinheiro", "Pix", etc
        public string DescricaoCache { get; set; } = null!;  // Exemplo: "1 hora", "Noite toda"
        public decimal ValorCache { get; set; }              // Exemplo: 200.00
    }
}
