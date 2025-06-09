// Models/VideoAnuncio.cs
namespace buzzaraApi.Models
{
    public class VideoAnuncio
    {
        public int VideoAnuncioID { get; set; }
        public string Url { get; set; } = null!;
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;

        public int ServicoID { get; set; }
        public Servico Servico { get; set; } = null!;
    }
}
