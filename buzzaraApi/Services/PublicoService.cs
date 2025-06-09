using buzzaraApi.Data;
using buzzaraApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Services
{
    public class PublicoService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _baseUrl;

        public PublicoService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _baseUrl = config["AppSettings:PublicBaseUrl"] ?? "https://api.buzzara.com.br";
        }

        public async Task<UsuarioPerfilDTO?> ObterPerfilPublico(int userId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.PerfisAcompanhantes)
                .FirstOrDefaultAsync(u => u.UsuarioID == userId && u.Ativo);

            if (usuario == null) return null;

            var perfil = usuario.PerfisAcompanhantes.FirstOrDefault();

            return new UsuarioPerfilDTO
            {
                UsuarioID = usuario.UsuarioID,
                PerfilAcompanhanteID = perfil?.PerfilAcompanhanteID ?? 0,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Genero = usuario.Genero,
                DataNascimento = usuario.DataNascimento,
                FotoPerfilUrl = MontarUrl(usuario.FotoPerfilUrl),
                FotoCapaUrl = MontarUrl(usuario.FotoCapaUrl),
                Localizacao = perfil?.Localizacao,
                Descricao = perfil?.Descricao,
                EstaOnline = usuario.EstaOnline,
                UltimoAcesso = usuario.UltimoAcesso,
                Tarifa = perfil?.Tarifa ?? 0
            };
        }

        private string? MontarUrl(string? caminhoRelativo)
        {
            if (string.IsNullOrWhiteSpace(caminhoRelativo)) return null;
            if (caminhoRelativo.StartsWith("http")) return caminhoRelativo;

            return $"{_baseUrl}{caminhoRelativo}";
        }
    }
}
