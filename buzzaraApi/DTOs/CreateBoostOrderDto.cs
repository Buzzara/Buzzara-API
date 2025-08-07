namespace buzzaraApi.DTOs
{
    public class CreateBoostOrderDto
    {
        public int ServicoId { get; set; }
        public int PlanId { get; set; }
        public DateOnly StartDate { get; set; }
        public TimeOnly FirstTime { get; set; }
        public TimeOnly LastTime { get; set; }
    }
}
