namespace buzzaraApi.Models
{
    public class VideoAcompanhante
    {
        public int VideoAcompanhanteID { get; set; }

        // FK para PerfilAcompanhante
        public int PerfilAcompanhanteID { get; set; }
        public PerfilAcompanhante PerfilAcompanhante { get; set; } = null!;

        public string UrlVideo { get; set; } = null!;
        public DateTime DataUpload { get; set; } = DateTime.Now;
    }
}
