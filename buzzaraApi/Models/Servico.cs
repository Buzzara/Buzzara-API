// Models/Servico.cs
namespace buzzaraApi.Models
{
    public class Servico
    {
        public int ServicoID { get; set; }
        public string Nome { get; set; } = null!;
        public string Saidas { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }

        public required string Categoria { get; set; }
        public required string LugarEncontro { get; set; }
        public required string ServicoPrestado { get; set; }
        public required string ServicoEspecial { get; set; }

        public string? Disponibilidade { get; set; }

        public int? Idade { get; set; }      
        public decimal? Peso { get; set; }         
        public decimal? Altura { get; set; }       

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;

        public int PerfilAcompanhanteID { get; set; }
        public PerfilAcompanhante PerfilAcompanhante { get; set; } = null!;

        public ICollection<FotoAnuncio> Fotos { get; set; } = new List<FotoAnuncio>();
        public ICollection<VideoAnuncio> Videos { get; set; } = new List<VideoAnuncio>();
        public Localizacao? Localizacao { get; set; }
        public SobreUsuario? SobreUsuario { get; set; }
        public ICollection<ServicoCache> Caches { get; set; } = new List<ServicoCache>();

    }

}
