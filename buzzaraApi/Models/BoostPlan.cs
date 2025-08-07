using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buzzaraApi.Models
{
    [Table("BoostPlans")]
    public class BoostPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public int DaysDuration { get; set; }

        [Required]
        public int UpsPerDay { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool Recommended { get; set; } = false;

        // ————————————————————————————————
        // Navegação inversa (opcional)
        // ————————————————————————————————
        public ICollection<BoostOrder> Orders { get; set; } = new List<BoostOrder>();
    }
}
