// DTOs/UploadFotoPerfilDTO.cs
namespace buzzaraApi.DTOs
{
    public class UploadFotoPerfilDTO
    {
        public IFormFile? FotoPerfil { get; set; }
        public IFormFile? FotoCapa { get; set; }
    }
}
