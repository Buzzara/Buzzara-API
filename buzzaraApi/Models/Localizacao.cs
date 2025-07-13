using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace buzzaraApi.Models
{
    public class Localizacao
    {
        [Key]
        public int LocalizacaoID { get; set; }

        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Bairro { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int ServicoID { get; set; }
        public Servico Servico { get; set; } = null!;
    }
}
