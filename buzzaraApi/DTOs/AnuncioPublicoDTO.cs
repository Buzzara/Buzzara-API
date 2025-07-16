// DTOs/AnuncioPublicoDTO.cs
using buzzaraApi.Models;

namespace buzzaraApi.DTOs
{
    public class AnuncioPublicoDTO
    {
        public int UsuarioID { get; set; }
        public int ServicoID { get; set; }
        public string Nome { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public string Saidas { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string LugarEncontro { get; set; } = null!;
        public required string ServicoPrestado { get; set; }
        public required string ServicoEspecial { get; set; }
        public string? Disponibilidade { get; set; }
        public int? Idade { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string? NomeAcompanhante { get; set; }

        public LocalizacaoDTO? Localizacao { get; set; }

        public List<FotoAnuncioDTO> Fotos { get; set; } = new();
        public List<VideoAnuncioDTO> Videos { get; set; } = new();
        public SobreUsuario? SobreUsuario { get; set; }
        public ICollection<ServicoCache> Caches { get; set; } = new List<ServicoCache>();
    }
}
