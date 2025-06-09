using buzzaraApi.Data;
using buzzaraApi.DTOs;
using System.Linq;

namespace buzzaraApi.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;
        public DashboardService(ApplicationDbContext context)
            => _context = context;

        public DashboardDataDTO ObterMetricasUsuario(int userId)
        {
            // 1) Pegar os IDs de perfil do usuário
            var perfilIds = _context.PerfisAcompanhantes
                .Where(p => p.UsuarioID == userId)
                .Select(p => p.PerfilAcompanhanteID);

            // 2) Pegar os IDs de serviço ativos desses perfis
            var servicoIds = _context.Servicos
                .Where(s => perfilIds.Contains(s.PerfilAcompanhanteID) && s.Ativo)
                .Select(s => s.ServicoID);

            // 3) Contagens
            var totalAnuncios = servicoIds.Count();
            var totalFotos = _context.FotosAnuncios
                                  .Count(f => servicoIds.Contains(f.ServicoID));
            var totalVideos = _context.VideosAnuncios
                                  .Count(v => servicoIds.Contains(v.ServicoID));

            // 4) Data do último upload de FOTO (pode adaptar para vídeo se quiser)
            var ultimoUpload = _context.FotosAnuncios
                .Where(f => servicoIds.Contains(f.ServicoID))
                .OrderByDescending(f => f.DataUpload)
                .Select(f => f.DataUpload)
                .FirstOrDefault();

            return new DashboardDataDTO
            {
                TotalAnuncios = totalAnuncios,
                TotalFotos = totalFotos,
                TotalVideos = totalVideos,
                UltimoUpload = ultimoUpload
            };
        }
    }
}
