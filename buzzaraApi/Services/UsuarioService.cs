using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Services
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _baseUrl;

        public UsuarioService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _baseUrl = config["AppSettings:PublicBaseUrl"] ?? "https://api.buzzara.com.br";
        }

        public async Task<Usuario> RegistrarUsuario(Usuario usuario)
        {
            // Hash da senha antes de salvar
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<List<Usuario>> ObterTodosUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> ObterUsuarioPorId(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null) return null;

            // Adiciona o domínio às URLs relativas
            user.FotoPerfilUrl = string.IsNullOrWhiteSpace(user.FotoPerfilUrl) ? null : $"{_baseUrl}{user.FotoPerfilUrl}";
            user.FotoCapaUrl = string.IsNullOrWhiteSpace(user.FotoCapaUrl) ? null : $"{_baseUrl}{user.FotoCapaUrl}";

            return user;
        }


        public async Task<Usuario?> AtualizarUsuario(int id, Usuario usuarioAtualizado)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
                return null;

            // Se a senha tiver sido alterada, re-hasheie
            if (usuarioAtualizado.SenhaHash != usuarioExistente.SenhaHash)
            {
                usuarioExistente.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioAtualizado.SenhaHash);
            }

            usuarioExistente.Nome = usuarioAtualizado.Nome;
            usuarioExistente.Email = usuarioAtualizado.Email;
            // ... outros campos

            await _context.SaveChangesAsync();
            return usuarioExistente;
        }

        public async Task<bool> DeletarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AtualizarFotosPerfilAsync(int userId, IFormFile? fotoPerfil, IFormFile? fotoCapa)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException("Usuário não encontrado.");

            var relativeFolder = Path.Combine("wwwroot", "uploads", "usuarios", userId.ToString());
            var fullFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            Directory.CreateDirectory(fullFolder);

            if (fotoPerfil != null)
            {
                if (!string.IsNullOrWhiteSpace(user.FotoPerfilUrl))
                {
                    var oldPath = Path.Combine("wwwroot", user.FotoPerfilUrl.TrimStart('/'));
                    var absoluteOld = Path.Combine(Directory.GetCurrentDirectory(), oldPath);
                    if (File.Exists(absoluteOld)) File.Delete(absoluteOld);
                }

                var fileName = $"perfil_{Guid.NewGuid()}{Path.GetExtension(fotoPerfil.FileName)}";
                var path = Path.Combine(fullFolder, fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await fotoPerfil.CopyToAsync(stream);

                user.FotoPerfilUrl = $"/uploads/usuarios/{userId}/{fileName}";
            }

            if (fotoCapa != null)
            {
                if (!string.IsNullOrWhiteSpace(user.FotoCapaUrl))
                {
                    var oldPath = Path.Combine("wwwroot", user.FotoCapaUrl.TrimStart('/'));
                    var absoluteOld = Path.Combine(Directory.GetCurrentDirectory(), oldPath);
                    if (File.Exists(absoluteOld)) File.Delete(absoluteOld);
                }

                var fileName = $"capa_{Guid.NewGuid()}{Path.GetExtension(fotoCapa.FileName)}";
                var path = Path.Combine(fullFolder, fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await fotoCapa.CopyToAsync(stream);

                user.FotoCapaUrl = $"/uploads/usuarios/{userId}/{fileName}";
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoverFotoAsync(int userId, string tipo)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException("Usuário não encontrado.");

            if (tipo == "perfil" && !string.IsNullOrWhiteSpace(user.FotoPerfilUrl))
            {
                var caminho = Path.Combine("wwwroot", user.FotoPerfilUrl.TrimStart('/'));
                if (File.Exists(caminho)) File.Delete(caminho);
                user.FotoPerfilUrl = null;
            }
            else if (tipo == "capa" && !string.IsNullOrWhiteSpace(user.FotoCapaUrl))
            {
                var caminho = Path.Combine("wwwroot", user.FotoCapaUrl.TrimStart('/'));
                if (File.Exists(caminho)) File.Delete(caminho);
                user.FotoCapaUrl = null;
            }
            else
            {
                throw new InvalidOperationException("Tipo inválido ou foto inexistente.");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PerfilUsuarioDTO?> ObterPerfilDoUsuario(int userId, string baseUrl)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.PerfisAcompanhantes)
                .FirstOrDefaultAsync(u => u.UsuarioID == userId);

            if (usuario == null)
                return null;

            var perfil = usuario.PerfisAcompanhantes.FirstOrDefault(); // Pega o primeiro perfil do usuário

            return new PerfilUsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Genero = usuario.Genero,
                UltimoAcesso = usuario.UltimoAcesso,
                UltimoIP = usuario.UltimoIP,
                EstaOnline = usuario.EstaOnline,
                DataNascimento = usuario.DataNascimento,
                FotoPerfilUrl = string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl) ? null : $"{baseUrl}{usuario.FotoPerfilUrl}",
                FotoCapaUrl = string.IsNullOrWhiteSpace(usuario.FotoCapaUrl) ? null : $"{baseUrl}{usuario.FotoCapaUrl}",

                // Dados do perfil acompanhante
                Descricao = perfil?.Descricao,
                Localizacao = perfil?.Localizacao
            };
        }
    }
}
