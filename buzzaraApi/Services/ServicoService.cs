using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;
using FFMpegCore;
using FFMpegCore.Pipes;



namespace buzzaraApi.Services
{
    public class ServicoService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly string _baseUrl;

        public ServicoService(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _baseUrl = config["AppSettings:PublicBaseUrl"] ?? "https://api.buzzara.com.br";
        }

        // Cria novo anúncio
        public async Task<ServicoDTO> CreateAsync(CreateServicoDTO dto, int userId)
        {
            var perfil = await _ctx.PerfisAcompanhantes
                .SingleOrDefaultAsync(p => p.UsuarioID == userId);

            if (perfil == null)
                throw new KeyNotFoundException("Perfil não encontrado.");

            var servico = new Servico
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                ServicoPrestado = dto.ServicoPrestado,
                ServicoEspecial = dto.ServicoEspecial,
                Preco = dto.Preco,
                Categoria = dto.Categoria,
                LugarEncontro = dto.LugarEncontro,
                Disponibilidade = dto.Disponibilidade,
                Idade = dto.Idade,
                Peso = dto.Peso,
                Altura = dto.Altura,
                PerfilAcompanhanteID = perfil.PerfilAcompanhanteID
            };


            _ctx.Servicos.Add(servico);
            await _ctx.SaveChangesAsync(); // Primeiro salva para obter o ID

            // Salvar localização, se informada
            if (!string.IsNullOrWhiteSpace(dto.Cidade) || !string.IsNullOrWhiteSpace(dto.Estado) || dto.Latitude != null)
            {
                var localizacao = new Localizacao
                {
                    ServicoID = servico.ServicoID,
                    Endereco = dto.Endereco,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Bairro = dto.Bairro,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude
                };

                _ctx.Localizacoes.Add(localizacao);
                await _ctx.SaveChangesAsync();
            }

            // Upload das fotos (máximo 4)
            if (dto.Fotos != null && dto.Fotos.Any())
            {
                if (dto.Fotos.Count > 4)
                    throw new InvalidOperationException("Máximo de 4 fotos permitidas no anúncio básico.");

                foreach (var foto in dto.Fotos)
                    await SalvarFoto(servico.ServicoID, userId, foto);
            }

            // Upload do vídeo (máximo 50 segundos)
            if (dto.Video != null)
            {
                var duracao = await ObterDuracaoVideoSegundos(dto.Video);
                if (duracao > 50)
                    throw new InvalidOperationException("O vídeo deve ter no máximo 50 segundos.");

                await SalvarVideo(servico.ServicoID, userId, dto.Video);
            }

            return new ServicoDTO
            {
                ServicoID = servico.ServicoID,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Preco = servico.Preco,
                Categoria = servico.Categoria,
                LugarEncontro = servico.LugarEncontro,
                Disponibilidade = servico.Disponibilidade,
                Idade = servico.Idade,
                Peso = servico.Peso,
                Altura = servico.Altura,
                DataCriacao = servico.DataCriacao
            };

        }

        // Lista todos os anúncios do usuário
        public async Task<List<ServicoDTO>> GetAllByUserAsync(int userId)
        {
            var servicos = await _ctx.Servicos
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Where(s => s.PerfilAcompanhante.UsuarioID == userId && s.Ativo)
                .ToListAsync();

            return servicos.Select(s => new ServicoDTO
            {
                ServicoID = s.ServicoID,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Preco = s.Preco,
                Categoria = s.Categoria,
                LugarEncontro = s.LugarEncontro,
                Disponibilidade = s.Disponibilidade,
                Idade = s.Idade,
                Peso = s.Peso,
                Altura = s.Altura,
                DataCriacao = s.DataCriacao,
                Fotos = s.Fotos.Select(f => new FotoAnuncioDTO
                {
                    FotoAnuncioID = f.FotoAnuncioID,
                    Url = $"{_baseUrl}{f.Url}",
                    DataUpload = f.DataUpload
                }).ToList(),
                Videos = s.Videos.Select(v => new VideoAnuncioDTO
                {
                    VideoAnuncioID = v.VideoAnuncioID,
                    Url = $"{_baseUrl}{v.Url}",
                    DataUpload = v.DataUpload
                }).ToList()
            }).ToList();

        }

        // Upload de nova foto para um anúncio existente
        public async Task<FotoAnuncioDTO> UploadFotoAsync(int servicoId, IFormFile file, int userId)
        {
            await ValidarPermissaoAnuncio(servicoId, userId);

            var foto = await SalvarFoto(servicoId, userId, file);

            return new FotoAnuncioDTO
            {
                FotoAnuncioID = foto.FotoAnuncioID,
                Url = $"{_baseUrl}{foto.Url}",
                DataUpload = foto.DataUpload
            };
        }

        // Upload de novo vídeo para um anúncio existente
        public async Task<VideoAnuncioDTO> UploadVideoAsync(int servicoId, IFormFile file, int userId)
        {
            await ValidarPermissaoAnuncio(servicoId, userId);

            var video = await SalvarVideo(servicoId, userId, file);

            return new VideoAnuncioDTO
            {
                VideoAnuncioID = video.VideoAnuncioID,
                Url = $"{_baseUrl}{video.Url}",
                DataUpload = video.DataUpload
            };
        }

        // ========== Métodos privados ==========

        private async Task ValidarPermissaoAnuncio(int servicoId, int userId)
        {
            var servico = await _ctx.Servicos
                .Include(s => s.PerfilAcompanhante)
                .FirstOrDefaultAsync(s => s.ServicoID == servicoId);

            if (servico == null)
                throw new KeyNotFoundException("Anúncio não encontrado.");

            if (servico.PerfilAcompanhante.UsuarioID != userId)
                throw new SecurityException("Não autorizado.");
        }

        private async Task<FotoAnuncio> SalvarFoto(int servicoId, int userId, IFormFile file)
        {
            var (path, url) = await SalvarArquivo(file, userId, servicoId);

            var foto = new FotoAnuncio
            {
                ServicoID = servicoId,
                Url = url
            };

            _ctx.FotosAnuncios.Add(foto);
            await _ctx.SaveChangesAsync();

            return foto;
        }

        private async Task<VideoAnuncio> SalvarVideo(int servicoId, int userId, IFormFile file)
        {
            var (path, url) = await SalvarArquivo(file, userId, servicoId);

            var video = new VideoAnuncio
            {
                ServicoID = servicoId,
                Url = url
            };

            _ctx.VideosAnuncios.Add(video);
            await _ctx.SaveChangesAsync();

            return video;
        }

        private async Task<(string path, string url)> SalvarArquivo(IFormFile file, int userId, int servicoId)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var folder = Path.Combine("wwwroot", "uploads", "anuncios", userId.ToString(), servicoId.ToString());
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folder); // Garantir caminho absoluto

            Directory.CreateDirectory(fullPath); // Cria pasta se não existir

            var filePath = Path.Combine(fullPath, fileName);

            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);

            var url = $"/uploads/anuncios/{userId}/{servicoId}/{fileName}";
            return (filePath, url);
        }


        private async Task<double> ObterDuracaoVideoSegundos(IFormFile file)
        {
            var tempFilePath = Path.GetTempFileName();

            await using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var mediaInfo = await FFProbe.AnalyseAsync(tempFilePath);

            File.Delete(tempFilePath);

            return mediaInfo.Duration.TotalSeconds;
        }
        public async Task<ServicoDTO> UpdateAsync(int servicoId, int userId, UpdateServicoDTO dto)
        {
            await ValidarPermissaoAnuncio(servicoId, userId);

            var servico = await _ctx.Servicos
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Include(s => s.Localizacao)
                .FirstOrDefaultAsync(s => s.ServicoID == servicoId)
                ?? throw new KeyNotFoundException("Anúncio não encontrado.");
            servico.Nome = dto.Nome;
            servico.Descricao = dto.Descricao;
            servico.Preco = dto.Preco;
            servico.Categoria = dto.Categoria;
            servico.LugarEncontro = dto.LugarEncontro;
            servico.Disponibilidade = dto.Disponibilidade;
            servico.DataAtualizacao = DateTime.UtcNow;


            // // Atualiza localização
            // if (servico.Localizacao == null)
            //     servico.Localizacao = new Localizacao { ServicoID = servicoId };

            // servico.Localizacao.Endereco = dto.Endereco;
            // servico.Localizacao.Cidade = dto.Cidade;
            // servico.Localizacao.Estado = dto.Estado;
            // servico.Localizacao.Bairro = dto.Bairro;
            // servico.Localizacao.Latitude = dto.Latitude;
            // servico.Localizacao.Longitude = dto.Longitude;

            // 1) Substitui fotos (se vieram novas)
            if (dto.NovasFotos != null)
            {
                // 1a) Deleta fisicamente cada foto antiga
                foreach (var foto in servico.Fotos)
                {
                    var physicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        foto.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                    );
                    if (File.Exists(physicalPath))
                        File.Delete(physicalPath);
                }

                // 1b) Remove do EF e salva essa exclusão
                _ctx.FotosAnuncios.RemoveRange(servico.Fotos);
                await _ctx.SaveChangesAsync();

                // 1c) Salva as novas
                foreach (var nova in dto.NovasFotos)
                    await SalvarFoto(servicoId, userId, nova);
            }

            // 2) Substitui vídeo (se vier novo)
            if (dto.NovoVideo != null)
            {
                // 2a) Deleta fisicamente cada vídeo antigo
                foreach (var video in servico.Videos)
                {
                    var physicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        video.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                    );
                    if (File.Exists(physicalPath))
                        File.Delete(physicalPath);
                }

                // 2b) Remove do EF e salva essa exclusão
                _ctx.VideosAnuncios.RemoveRange(servico.Videos);
                await _ctx.SaveChangesAsync();

                // 2c) Valida duração e salva o novo
                var duracao = await ObterDuracaoVideoSegundos(dto.NovoVideo);
                if (duracao > 50)
                    throw new InvalidOperationException("O vídeo deve ter no máximo 50 segundos.");

                await SalvarVideo(servicoId, userId, dto.NovoVideo);
            }

            // 3) (Opcional) Atualize localização aqui...

            // 4) Persiste data de atualização em campo DataAtualizacao
            await _ctx.SaveChangesAsync();

            // 5) Retorna o DTO atualizado
            return await GetAllByUserAsync(userId)
                .ContinueWith(t => t.Result.First(s => s.ServicoID == servicoId));
        }

        public async Task<bool> DeleteAsync(int servicoId, int userId)
        {
            await ValidarPermissaoAnuncio(servicoId, userId);

            var servico = await _ctx.Servicos
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Include(s => s.Localizacao)
                .FirstOrDefaultAsync(s => s.ServicoID == servicoId);

            if (servico == null)
                return false;

            // Remove arquivos associados
            _ctx.FotosAnuncios.RemoveRange(servico.Fotos);
            _ctx.VideosAnuncios.RemoveRange(servico.Videos);
            if (servico.Localizacao != null)
                _ctx.Localizacoes.Remove(servico.Localizacao);

            _ctx.Servicos.Remove(servico);
            await _ctx.SaveChangesAsync();
            return true;
        }

    }
}
