namespace buzzaraApi.DTOs
{
    public class CreatePreferenceDto
    {
        public string Title { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // Se quiser mensagens customizadas:
        public string? PayerEmail { get; set; }
    }
}
