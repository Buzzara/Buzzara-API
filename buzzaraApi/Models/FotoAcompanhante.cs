namespace buzzaraApi.Models
{
    public class FotoAcompanhante
    {
        public int FotoAcompanhanteID { get; set; }

        // FK para PerfilAcompanhante
        public int PerfilAcompanhanteID { get; set; }
        public PerfilAcompanhante PerfilAcompanhante { get; set; } = null!;

        public string UrlFoto { get; set; } = null!;
        public DateTime DataUpload { get; set; } = DateTime.Now;
    }
}
