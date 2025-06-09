//using buzzaraApi.Data;
//using buzzaraApi.DTOs;
//using buzzaraApi.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging; // Para logging (opcional)

//namespace buzzaraApi.Services
//{
//    public class MidiaService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly ILogger<MidiaService> _logger; // Opcional: para logging
//        private readonly string[] _allowedPhotoExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Extensões permitidas
//        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB (tamanho máximo do arquivo)

//        public MidiaService(
//            ApplicationDbContext context,
//            IHttpContextAccessor httpContextAccessor,
//            ILogger<MidiaService> logger = null) // Logger é opcional
//        {
//            _context = context;
//            _httpContextAccessor = httpContextAccessor;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Adiciona uma foto a partir de uma URL fornecida no DTO.
//        /// </summary>
//        public async Task<FotoAcompanhanteDTO> AdicionarFoto(FotoAcompanhanteDTO dto)
//        {
//            // Validar o usuário autenticado
//            var userId = GetAuthenticatedUserId();
//            if (userId == null)
//            {
//                _logger?.LogWarning("Tentativa de adicionar foto sem usuário autenticado.");
//                throw new UnauthorizedAccessException("Usuário não autenticado.");
//            }

//            // Verificar se o perfil existe e pertence ao usuário autenticado
//            var perfil = await _context.PerfisAcompanhantes
//                .FirstOrDefaultAsync(p => p.PerfilAcompanhanteID == dto.PerfilAcompanhanteID && p.UsuarioID == userId);
//            if (perfil == null)
//            {
//                _logger?.LogWarning($"Perfil {dto.PerfilAcompanhanteID} não encontrado ou não pertence ao usuário {userId}.");
//                throw new KeyNotFoundException("Perfil não encontrado ou você não tem permissão para adicionar fotos a este perfil.");
//            }

//            // Validar a URL da foto (opcional: você pode adicionar mais validações)
//            if (string.IsNullOrWhiteSpace(dto.UrlFoto))
//            {
//                throw new ArgumentException("A URL da foto não pode estar vazia.");
//            }

//            // Criar a entidade FotoAcompanhante
//            var foto = new FotoAcompanhante
//            {
//                PerfilAcompanhanteID = dto.PerfilAcompanhanteID,
//                UrlFoto = dto.UrlFoto,
//                DataUpload = DateTime.UtcNow
//            };

//            _context.FotosAcompanhantes.Add(foto);
//            await _context.SaveChangesAsync();

//            _logger?.LogInformation($"Foto adicionada com sucesso para o perfil {dto.PerfilAcompanhanteID} pelo usuário {userId}.");

//            // Retornar o DTO
//            return new FotoAcompanhanteDTO
//            {
//                PerfilAcompanhanteID = foto.PerfilAcompanhanteID,
//                UrlFoto = foto.UrlFoto
//            };
//        }

//        /// <summary>
//        /// Adiciona um vídeo a partir de uma URL fornecida no DTO.
//        /// </summary>
//        public async Task<VideoAcompanhanteDTO> AdicionarVideo(VideoAcompanhanteDTO dto)
//        {
//            // Validar o usuário autenticado
//            var userId = GetAuthenticatedUserId();
//            if (userId == null)
//            {
//                _logger?.LogWarning("Tentativa de adicionar vídeo sem usuário autenticado.");
//                throw new UnauthorizedAccessException("Usuário não autenticado.");
//            }

//            // Verificar se o perfil existe e pertence ao usuário autenticado
//            var perfil = await _context.PerfisAcompanhantes
//                .FirstOrDefaultAsync(p => p.PerfilAcompanhanteID == dto.PerfilAcompanhanteID && p.UsuarioID == userId);
//            if (perfil == null)
//            {
//                _logger?.LogWarning($"Perfil {dto.PerfilAcompanhanteID} não encontrado ou não pertence ao usuário {userId}.");
//                throw new KeyNotFoundException("Perfil não encontrado ou você não tem permissão para adicionar vídeos a este perfil.");
//            }

//            // Validar a URL do vídeo (opcional: você pode adicionar mais validações)
//            if (string.IsNullOrWhiteSpace(dto.UrlVideo))
//            {
//                throw new ArgumentException("A URL do vídeo não pode estar vazia.");
//            }

//            // Criar a entidade VideoAcompanhante
//            var video = new VideoAcompanhante
//            {
//                PerfilAcompanhanteID = dto.PerfilAcompanhanteID,
//                UrlVideo = dto.UrlVideo,
//                DataUpload = DateTime.UtcNow
//            };

//            _context.VideosAcompanhantes.Add(video);
//            await _context.SaveChangesAsync();

//            _logger?.LogInformation($"Vídeo adicionado com sucesso para o perfil {dto.PerfilAcompanhanteID} pelo usuário {userId}.");

//            // Retornar o DTO
//            return new VideoAcompanhanteDTO
//            {
//                PerfilAcompanhanteID = video.PerfilAcompanhanteID,
//                UrlVideo = video.UrlVideo
//            };
//        }

//        /// <summary>
//        /// Faz o upload de uma foto e a associa a um perfil.
//        /// </summary>
//        public async Task<FotoAcompanhanteDTO> UploadFoto(IFormFile file, int perfilAcompanhanteID)
//        {
//            // Validar o usuário autenticado
//            var userId = GetAuthenticatedUserId();
//            if (userId == null)
//            {
//                _logger?.LogWarning("Tentativa de upload de foto sem usuário autenticado.");
//                throw new UnauthorizedAccessException("Usuário não autenticado.");
//            }

//            // Verificar se o perfil existe e pertence ao usuário autenticado
//            var perfil = await _context.PerfisAcompanhantes
//                .FirstOrDefaultAsync(p => p.PerfilAcompanhanteID == perfilAcompanhanteID && p.UsuarioID == userId);
//            if (perfil == null)
//            {
//                _logger?.LogWarning($"Perfil {perfilAcompanhanteID} não encontrado ou não pertence ao usuário {userId}.");
//                throw new KeyNotFoundException("Perfil não encontrado ou você não tem permissão para adicionar fotos a este perfil.");
//            }

//            // Validar o arquivo
//            if (file == null || file.Length == 0)
//            {
//                throw new ArgumentException("Nenhum arquivo foi enviado.");
//            }

//            if (file.Length > _maxFileSize)
//            {
//                throw new ArgumentException($"O arquivo excede o tamanho máximo permitido de {_maxFileSize / (1024 * 1024)} MB.");
//            }

//            var extension = Path.GetExtension(file.FileName).ToLower();
//            if (!_allowedPhotoExtensions.Contains(extension))
//            {
//                throw new ArgumentException($"Tipo de arquivo não permitido. Extensões permitidas: {string.Join(", ", _allowedPhotoExtensions)}.");
//            }

//            // Fazer o upload do arquivo
//            var fileUrl = await SaveFileAsync(file, "Fotos");

//            // Criar a entidade FotoAcompanhante
//            var foto = new FotoAcompanhante
//            {
//                PerfilAcompanhanteID = perfilAcompanhanteID,
//                UrlFoto = fileUrl,
//                DataUpload = DateTime.UtcNow
//            };

//            _context.FotosAcompanhantes.Add(foto);
//            await _context.SaveChangesAsync();

//            _logger?.LogInformation($"Foto carregada com sucesso para o perfil {perfilAcompanhanteID} pelo usuário {userId}. URL: {fileUrl}");

//            // Retornar o DTO
//            return new FotoAcompanhanteDTO
//            {
//                PerfilAcompanhanteID = foto.PerfilAcompanhanteID,
//                UrlFoto = foto.UrlFoto
//            };
//        }

//        /// <summary>
//        /// Método auxiliar para salvar arquivos no servidor.
//        /// </summary>
//        private async Task<string> SaveFileAsync(IFormFile file, string subfolder)
//        {
//            try
//            {
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", subfolder);
//                if (!Directory.Exists(folderPath))
//                {
//                    Directory.CreateDirectory(folderPath);
//                }

//                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//                var filePath = Path.Combine(folderPath, fileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                return $"/Uploads/{subfolder}/{fileName}";
//            }
//            catch (Exception ex)
//            {
//                _logger?.LogError(ex, $"Erro ao salvar arquivo no servidor. Subpasta: {subfolder}");
//                throw new IOException("Erro ao salvar o arquivo no servidor.", ex);
//            }
//        }

//        /// <summary>
//        /// Obtém o ID do usuário autenticado a partir do token JWT.
//        /// </summary>
//        private int? GetAuthenticatedUserId()
//        {
//            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
//            {
//                return null;
//            }
//            return userId;
//        }
//    }
//}