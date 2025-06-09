// DTOs/AnuncioPublicoDTO.cs
namespace buzzaraApi.DTOs
{
    public class AnuncioPublicoDTO
    {
        public int UsuarioID { get; set; }
        public int ServicoID { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }
        public string Categoria { get; set; } = null!;
        public string LugarEncontro { get; set; } = null!;
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
    }
}
