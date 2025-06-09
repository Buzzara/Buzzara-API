using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buzzaraApi.Models
{
    [Table("PagamentosAnuncios")]
    public class PagamentoAnuncio
    {
        [Key]
        public int PagamentoAnuncioID { get; set; }

        [Required]
        public int ServicoID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        [Required]
        public bool Pago { get; set; } = false;

        [Required]
        [MaxLength(500)]
        public string QrCodeUrl { get; set; } = null!;

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Relacionamento com Servico (FK)
        [ForeignKey(nameof(ServicoID))]
        public Servico Servico { get; set; } = null!;
    }
}
