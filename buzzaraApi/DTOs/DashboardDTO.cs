using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.DTOs
{
    public class DashboardDataDTO
    {
        public int TotalFotos { get; set; }
        public int TotalVideos { get; set; }
        public int TotalAnuncios { get; set; }
        public DateTime UltimoUpload { get; set; }
        // adicione aqui quaisquer outras métricas
    }
}
