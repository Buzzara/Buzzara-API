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
                LugarEncontro = dto.LugarEncontro,
                Disponibilidade = dto.Disponibilidade,
                Idade = dto.Idade,
                Peso = dto.Peso,
                Altura = dto.Altura,
                Saidas = FormatSaidas(dto.Saidas),
                PerfilAcompanhanteID = perfil.PerfilAcompanhanteID
            };

            _ctx.Servicos.Add(servico);
            await _ctx.SaveChangesAsync(); // Primeiro salva para obter o ID

            // ==========================
            // Salvar SobreUsuario
            // ==========================
            if (dto.SobreUsuario != null)
            {
                var sobreUsuario = new SobreUsuario
                {
                    ServicoId = servico.ServicoID,
                    Atendimento = dto.SobreUsuario.Atendimento,
                    Etnia = dto.SobreUsuario.Etnia,
                    Relacionamento = dto.SobreUsuario.Relacionamento,
                    Cabelo = dto.SobreUsuario.Cabelo,
                    Estatura = dto.SobreUsuario.Estatura,
                    Corpo = dto.SobreUsuario.Corpo,
                    Seios = dto.SobreUsuario.Seios,
                    Pubis = dto.SobreUsuario.Pubis
                };

                _ctx.SobreUsuarios.Add(sobreUsuario);
                await _ctx.SaveChangesAsync();
            }

            // ==========================
            // Salvar Caches
            // ==========================
            if (dto.Caches != null && dto.Caches.Any())
            {
                foreach (var cacheDto in dto.Caches)
                {
                    var cache = new ServicoCache
                    {
                        ServicoId = servico.ServicoID,
                        FormaPagamento = cacheDto.FormaPagamento,
                        DescricaoCache = cacheDto.Descricao,
                        ValorCache = cacheDto.Valor
                    };

                    _ctx.ServicosCaches.Add(cache);
                }

                await _ctx.SaveChangesAsync();
            }


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

            // Salvar horários de atendimento
            var horarios = MapearHorarios(new HorarioInputDTO
            {
                MesmoHorarioTodosOsDias = dto.MesmoHorarioTodosOsDias,
                HorarioUnico = dto.HorarioUnico,
                HorariosIndividuais = dto.HorariosIndividuais
            }, servico.ServicoID);

            if (horarios.Any())
            {
                _ctx.HorariosAtendimentos.AddRange(horarios);
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
                Saidas = servico.Saidas,
                LugarEncontro = servico.LugarEncontro,
                Disponibilidade = servico.Disponibilidade,
                Idade = servico.Idade,
                Peso = servico.Peso,
                Altura = servico.Altura,
                DataCriacao = servico.DataCriacao,
                HorariosAtendimento = horarios.Select(h => new HorarioAtendimentoDTO
                {
                    DiaSemana = h.DiaSemana,
                    Atende = h.Atende,
                    HorarioInicio = h.HorarioInicio?.ToString(@"hh\:mm"),
                    HorarioFim = h.HorarioFim?.ToString(@"hh\:mm"),
                    VinteQuatroHoras = h.VinteQuatroHoras
                }).ToList()

            };

        }

        // Lista todos os anúncios do usuário
        public async Task<List<ServicoDTO>> GetAllByUserAsync(int userId)
        {
            var servicos = await _ctx.Servicos
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Include(s => s.Localizacao)
                .Include(s => s.HorariosAtendimento)
                .Include(s => s.SobreUsuario)
                .Include(s => s.Caches)
                .Where(s => s.PerfilAcompanhante.UsuarioID == userId && s.Ativo)
                .ToListAsync();

            return servicos.Select(s =>
            {
                // Monta o DTO de SobreUsuario com segurança contra nulos
                var sobre = s.SobreUsuario != null
                    ? new SobreUsuarioDTO
                    {
                        Atendimento = s.SobreUsuario.Atendimento?.ToList() ?? new List<string>(),
                        Etnia = s.SobreUsuario.Etnia,
                        Relacionamento = s.SobreUsuario.Relacionamento,
                        Cabelo = s.SobreUsuario.Cabelo,
                        Estatura = s.SobreUsuario.Estatura,
                        Corpo = s.SobreUsuario.Corpo,
                        Seios = s.SobreUsuario.Seios,
                        Pubis = s.SobreUsuario.Pubis
                    }
                    : null;

                return new ServicoDTO
                {
                    ServicoID = s.ServicoID,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    Saidas = s.Saidas,
                    LugarEncontro = s.LugarEncontro,
                    Disponibilidade = s.Disponibilidade,
                    Idade = s.Idade,
                    Peso = s.Peso,
                    Altura = s.Altura,
                    DataCriacao = s.DataCriacao,

                    Localizacao = s.Localizacao == null ? null : new LocalizacaoDTO
                    {
                        Endereco = s.Localizacao.Endereco,
                        Cidade = s.Localizacao.Cidade,
                        Estado = s.Localizacao.Estado,
                        Bairro = s.Localizacao.Bairro,
                        Latitude = s.Localizacao.Latitude,
                        Longitude = s.Localizacao.Longitude
                    },
                    HorariosAtendimento = s.HorariosAtendimento?.Select(h => new HorarioAtendimentoDTO
                    {
                        DiaSemana = h.DiaSemana,
                        Atende = h.Atende,
                        HorarioInicio = h.HorarioInicio?.ToString(@"hh\:mm"),
                        HorarioFim = h.HorarioFim?.ToString(@"hh\:mm"),
                        VinteQuatroHoras = h.VinteQuatroHoras
                    }).ToList(),

                    SobreUsuario = sobre,

                    Caches = s.Caches.Select(c => new ServicoCacheDTO
                    {
                        FormaPagamento = c.FormaPagamento,
                        Descricao = c.DescricaoCache,
                        Valor = c.ValorCache
                    }).ToList(),

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
                };
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

        private string FormatSaidas(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Vou até lugar não informado.";

            var destinos = input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return destinos.Count switch
            {
                0 => "Vou até lugar não informado.",
                1 => $"Vou até {destinos[0]}.",
                2 => $"Vou até {destinos[0]} e {destinos[1]}.",
                _ => $"Vou até {string.Join(", ", destinos.Take(destinos.Count - 1))} e {destinos.Last()}."
            };
        }

        public async Task<ServicoDTO> UpdateAsync(int servicoId, int userId, UpdateServicoDTO dto)
        {
            await ValidarPermissaoAnuncio(servicoId, userId);

            // carrega com tudo
            var servico = await _ctx.Servicos
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Include(s => s.Localizacao)
                .Include(s => s.SobreUsuario)
                .Include(s => s.Caches)
                .Include(s => s.HorariosAtendimento)
                .FirstOrDefaultAsync(s => s.ServicoID == servicoId)
                ?? throw new KeyNotFoundException("Anúncio não encontrado.");

            // 1) campos básicos
            servico.Nome = dto.Nome;
            servico.Descricao = dto.Descricao;
            servico.LugarEncontro = dto.LugarEncontro;
            servico.Disponibilidade = dto.Disponibilidade;
            servico.Idade = dto.Idade;
            servico.Peso = dto.Peso;
            servico.Altura = dto.Altura;
            servico.Saidas = FormatSaidas(dto.Saidas);
            servico.DataAtualizacao = DateTime.UtcNow;

            // 2) Localização
            if (servico.Localizacao != null)
                _ctx.Localizacoes.Remove(servico.Localizacao);

            if (!string.IsNullOrWhiteSpace(dto.Cidade) || dto.Latitude != null)
            {
                _ctx.Localizacoes.Add(new Localizacao
                {
                    ServicoID = servicoId,
                    Endereco = dto.Endereco,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Bairro = dto.Bairro,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude
                });
            }

            // 3) Mídias
            if (dto.NovasFotos?.Any() == true)
            {
                foreach (var f in servico.Fotos)
                    File.Delete(Path.Combine("wwwroot", f.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
                _ctx.FotosAnuncios.RemoveRange(servico.Fotos);
                foreach (var f in dto.NovasFotos)
                    await SalvarFoto(servicoId, userId, f);
            }

            if (dto.NovoVideo != null)
            {
                foreach (var v in servico.Videos)
                    File.Delete(Path.Combine("wwwroot", v.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
                _ctx.VideosAnuncios.RemoveRange(servico.Videos);

                var dur = await ObterDuracaoVideoSegundos(dto.NovoVideo);
                if (dur > 50) throw new InvalidOperationException("Vídeo >50s");
                await SalvarVideo(servicoId, userId, dto.NovoVideo);
            }

            // 4) SobreUsuario
            if (servico.SobreUsuario != null)
                _ctx.SobreUsuarios.Remove(servico.SobreUsuario);

            if (dto.SobreUsuario != null)
            {
                _ctx.SobreUsuarios.Add(new SobreUsuario
                {
                    ServicoId = servicoId,
                    Atendimento = dto.SobreUsuario.Atendimento,
                    Etnia = dto.SobreUsuario.Etnia,
                    Relacionamento = dto.SobreUsuario.Relacionamento,
                    Cabelo = dto.SobreUsuario.Cabelo,
                    Estatura = dto.SobreUsuario.Estatura,
                    Corpo = dto.SobreUsuario.Corpo,
                    Seios = dto.SobreUsuario.Seios,
                    Pubis = dto.SobreUsuario.Pubis
                });
            }

            // 5) Caches
            if (servico.Caches.Any())
                _ctx.ServicosCaches.RemoveRange(servico.Caches);

            if (dto.Caches?.Any() == true)
            {
                foreach (var c in dto.Caches)
                    _ctx.ServicosCaches.Add(new ServicoCache
                    {
                        ServicoId = servicoId,
                        FormaPagamento = c.FormaPagamento,
                        DescricaoCache = c.Descricao,
                        ValorCache = c.Valor
                    });
            }

            // 6) Horários de Atendimento
            if (servico.HorariosAtendimento.Any())
                _ctx.HorariosAtendimentos.RemoveRange(servico.HorariosAtendimento);

            var novosHorarios = MapearHorarios(new HorarioInputDTO
            {
                MesmoHorarioTodosOsDias = dto.MesmoHorarioTodosOsDias,
                HorarioUnico = dto.HorarioUnico,
                HorariosIndividuais = dto.HorariosIndividuais
            }, servicoId);

            if (novosHorarios.Any())
                _ctx.HorariosAtendimentos.AddRange(novosHorarios);

            // 7) Salvar tudo
            await _ctx.SaveChangesAsync();

            // 8) Retornar atualizado
            return (await GetAllByUserAsync(userId)).First(x => x.ServicoID == servicoId);
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
        private static readonly string[] DiasDaSemana = new[]
        {
            "Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado", "Domingo"
        };

        private List<HorarioAtendimento> MapearHorarios(HorarioInputDTO dto, int servicoId)
        {
            var horarios = new List<HorarioAtendimento>();

            if (dto.MesmoHorarioTodosOsDias && dto.HorarioUnico != null)
            {
                foreach (var dia in DiasDaSemana)
                {
                    horarios.Add(new HorarioAtendimento
                    {
                        ServicoId = servicoId,
                        DiaSemana = dia,
                        Atende = dto.HorarioUnico.Atende,
                        HorarioInicio = TimeSpan.TryParse(dto.HorarioUnico.HorarioInicio, out var inicio) ? inicio : null,
                        HorarioFim = TimeSpan.TryParse(dto.HorarioUnico.HorarioFim, out var fim) ? fim : null,
                        VinteQuatroHoras = dto.HorarioUnico.VinteQuatroHoras
                    });
                }
            }
            else if (dto.HorariosIndividuais != null)
            {
                foreach (var h in dto.HorariosIndividuais)
                {
                    horarios.Add(new HorarioAtendimento
                    {
                        ServicoId = servicoId,
                        DiaSemana = h.DiaSemana!,
                        Atende = h.Atende,
                        HorarioInicio = TimeSpan.TryParse(h.HorarioInicio, out var inicioInd) ? inicioInd : null,
                        HorarioFim = TimeSpan.TryParse(h.HorarioFim, out var fimInd) ? fimInd : null,
                        VinteQuatroHoras = h.VinteQuatroHoras
                    });
                }
            }

            return horarios;
        }
    }
}
