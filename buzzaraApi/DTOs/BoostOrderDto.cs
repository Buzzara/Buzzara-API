namespace buzzaraApi.DTOs
{
    public class BoostOrderDto
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string InitPoint { get; set; } = null!;
        public string SandboxInitPoint { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
