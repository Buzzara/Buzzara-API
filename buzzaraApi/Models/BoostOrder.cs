using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buzzaraApi.Models
{
    [Table("BoostOrders")]
    public class BoostOrder
    {
        [Key]
        public int OrderId { get; set; }

        // → chave FK para Servico
        [Required]
        public int ServicoId { get; set; }

        // → chave FK para Usuario
        [Required]
        public int UserId { get; set; }

        // → chave FK para BoostPlan
        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public TimeSpan FirstTime { get; set; }
        [Required]
        public TimeSpan LastTime { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [MaxLength(100)]
        public string PreferenceId { get; set; } = null!;
        [MaxLength(500)]
        public string InitPoint { get; set; } = null!;
        [MaxLength(500)]
        public string SandboxInitPoint { get; set; } = null!;

        public bool Paid { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ————————————————————————————————
        // → Propriedades de navegação
        // ————————————————————————————————

        [ForeignKey(nameof(ServicoId))]
        public Servico Servico { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public Usuario User { get; set; } = null!;

        [ForeignKey(nameof(PlanId))]
        public BoostPlan Plan { get; set; } = null!;
    }
}
