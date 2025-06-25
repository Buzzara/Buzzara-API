// DTOs/CreateServicoDTO.cs
using buzzaraApi.Models;

namespace buzzaraApi.DTOs
{
    public class CreateServicoDTO
    {
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }
        public string Saidas { get; set; } = null!;

        public required string Categoria { get; set; }
        public required string LugarEncontro { get; set; }
        public required string ServicoPrestado { get; set; }
        public required string ServicoEspecial { get; set; }
        public string? Disponibilidade { get; set; }

        public int? Idade { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }

        // Localização diretamente no DTO
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Bairro { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<IFormFile>? Fotos { get; set; }
        public IFormFile? Video { get; set; }
        public SobreUsuarioDTO? SobreUsuario { get; set; }
        public List<ServicoCacheDTO> Caches { get; set; } = new List<ServicoCacheDTO>();

    }


}

// DTOs/ServicoDTO.cs
namespace buzzaraApi.DTOs
{
    public class ServicoDTO
    {
        public int ServicoID { get; set; }
        public string Saidas { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal Preco { get; set; }
        public required string Categoria { get; set; }
        public required string LugarEncontro { get; set; }
        public string? Disponibilidade { get; set; }
        public int? Idade { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public DateTime DataCriacao { get; set; }
        public LocalizacaoDTO? Localizacao { get; set; }
        public List<FotoAnuncioDTO> Fotos { get; set; } = new();
        public List<VideoAnuncioDTO> Videos { get; set; } = new();
        public SobreUsuarioDTO SobreUsuario { get; set; }
        public List<ServicoCacheDTO> Caches { get; set; }
    }
}

// DTOs/UploadFotoAnuncioDTO.cs
namespace buzzaraApi.DTOs
{
    public class UploadFotoAnuncioDTO
    {
        public IFormFile File { get; set; } = null!;
    }
}

// DTOs/FotoAnuncioDTO.cs
namespace buzzaraApi.DTOs
{
    public class FotoAnuncioDTO
    {
        public int FotoAnuncioID { get; set; }
        public string Url { get; set; } = null!;
        public DateTime DataUpload { get; set; }
    }
}

// DTOs/UploadVideoAnuncioDTO.cs
namespace buzzaraApi.DTOs
{
    public class UploadVideoAnuncioDTO
    {
        public IFormFile File { get; set; } = null!;
    }
}

// DTOs/VideoAnuncioDTO.cs
namespace buzzaraApi.DTOs
{
    public class VideoAnuncioDTO
    {
        public int VideoAnuncioID { get; set; }
        public string Url { get; set; } = null!;
        public DateTime DataUpload { get; set; }
    }
}
