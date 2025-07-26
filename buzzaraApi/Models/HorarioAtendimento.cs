using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buzzaraApi.Models
{
    public class HorarioAtendimento
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Servico")]
        public int ServicoId { get; set; }

        [Required]
        public string DiaSemana { get; set; } = string.Empty;

        public TimeSpan? HorarioInicio { get; set; }
        public TimeSpan? HorarioFim { get; set; }

        public bool Atende { get; set; }
        public bool VinteQuatroHoras { get; set; }

        public Servico? Servico { get; set; }
    }
}
