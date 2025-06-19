using buzzaraApi.Data;
using buzzaraApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Services
{
    public class AnuncioPublicoService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly string _baseUrl;

        public AnuncioPublicoService(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _baseUrl = config["AppSettings:PublicBaseUrl"] ?? "https://api.buzzara.com.br";
        }

        public async Task<List<AnuncioPublicoDTO>> ListarTodos(string? cidade = null, string? estado = null, string? bairro = null)
        {
            var query = _ctx.Servicos
                .Include(s => s.PerfilAcompanhante).ThenInclude(p => p.Usuario)
                .Include(s => s.Fotos)
                .Include(s => s.Videos)
                .Include(s => s.Localizacao)
                .Where(s => s.Ativo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(s => s.Localizacao != null && s.Localizacao.Cidade != null &&
                                         s.Localizacao.Cidade.ToLower().Contains(cidade.ToLower()));

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(s => s.Localizacao != null && s.Localizacao.Estado != null &&
                                         s.Localizacao.Estado.ToLower().Contains(estado.ToLower()));

            if (!string.IsNullOrEmpty(bairro))
                query = query.Where(s => s.Localizacao != null && s.Localizacao.Bairro != null &&
                                         s.Localizacao.Bairro.ToLower().Contains(bairro.ToLower()));

            var anuncios = await query.ToListAsync();

            return anuncios.Select(s => new AnuncioPublicoDTO
            {
                UsuarioID = s.PerfilAcompanhante.Usuario.UsuarioID,
                ServicoID = s.ServicoID,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Preco = s.Preco,
                DataCriacao = s.DataCriacao,

                Categoria = s.Categoria,
                LugarEncontro = s.LugarEncontro,
                Disponibilidade = s.Disponibilidade,
                Idade = s.Idade,
                Peso = s.Peso,
                Altura = s.Altura,

                ServicoPrestado = s.ServicoPrestado, // Fix for required property
                ServicoEspecial = s.ServicoEspecial,  // Fix for required property

                NomeAcompanhante = s.PerfilAcompanhante.Usuario.Nome,
                FotoPerfilUrl = MontarUrl(s.PerfilAcompanhante.Usuario.FotoPerfilUrl),

                Localizacao = s.Localizacao != null ? new LocalizacaoDTO
                {
                    Endereco = s.Localizacao.Endereco,
                    Bairro = s.Localizacao.Bairro,
                    Cidade = s.Localizacao.Cidade,
                    Estado = s.Localizacao.Estado,
                    Latitude = s.Localizacao.Latitude,
                    Longitude = s.Localizacao.Longitude
                } : null,

                Fotos = s.Fotos.Select(f => new FotoAnuncioDTO
                {
                    FotoAnuncioID = f.FotoAnuncioID,
                    Url = MontarUrl(f.Url),
                    DataUpload = f.DataUpload
                }).ToList(),

                Videos = s.Videos.Select(v => new VideoAnuncioDTO
                {
                    VideoAnuncioID = v.VideoAnuncioID,
                    Url = MontarUrl(v.Url),
                    DataUpload = v.DataUpload
                }).ToList()

            }).ToList();
        }

        public async Task<AnuncioPublicoDTO?> ObterPorIdAsync(int servicoId)
        {
            var s = await _ctx.Servicos
                .Include(x => x.PerfilAcompanhante).ThenInclude(p => p.Usuario)
                .Include(x => x.Fotos)
                .Include(x => x.Videos)
                .Include(x => x.Localizacao)
                .FirstOrDefaultAsync(x => x.ServicoID == servicoId && x.Ativo);

            if (s == null) return null;

            return new AnuncioPublicoDTO
            {
                UsuarioID = s.PerfilAcompanhante.Usuario.UsuarioID,
                ServicoID = s.ServicoID,
                Nome = s.Nome,
                Descricao = s.Descricao,
                Preco = s.Preco,
                DataCriacao = s.DataCriacao,
                Categoria = s.Categoria,
                LugarEncontro = s.LugarEncontro,
                Disponibilidade = s.Disponibilidade,
                ServicoPrestado = s.ServicoPrestado, // Fix for required property
                ServicoEspecial = s.ServicoEspecial,  // Fix for required property
                Idade = s.Idade,
                Peso = s.Peso,
                Altura = s.Altura,
                NomeAcompanhante = s.PerfilAcompanhante.Usuario.Nome,
                FotoPerfilUrl = MontarUrl(s.PerfilAcompanhante.Usuario.FotoPerfilUrl),
                Localizacao = s.Localizacao != null
                                    ? new LocalizacaoDTO
                                    {
                                        Endereco = s.Localizacao.Endereco,
                                        Bairro = s.Localizacao.Bairro,
                                        Cidade = s.Localizacao.Cidade,
                                        Estado = s.Localizacao.Estado,
                                        Latitude = s.Localizacao.Latitude,
                                        Longitude = s.Localizacao.Longitude
                                    }
                                    : null,
                Fotos = s.Fotos.Select(f => new FotoAnuncioDTO
                {
                    FotoAnuncioID = f.FotoAnuncioID,
                    Url = MontarUrl(f.Url),
                    DataUpload = f.DataUpload
                }).ToList(),
                Videos = s.Videos.Select(v => new VideoAnuncioDTO
                {
                    VideoAnuncioID = v.VideoAnuncioID,
                    Url = MontarUrl(v.Url),
                    DataUpload = v.DataUpload
                }).ToList()
            };
        }

        public async Task<List<AnuncioPublicoDTO>> ListarPorUsuario(int usuarioId)
        {
            return await ListarTodos()
                .ContinueWith(t => t.Result
                    .Where(x => x.FotoPerfilUrl != null && x.FotoPerfilUrl.Contains($"/{usuarioId}/"))
                    .ToList());
        }

        private string? MontarUrl(string? caminhoRelativo)
        {
            if (string.IsNullOrWhiteSpace(caminhoRelativo)) return null;

            if (caminhoRelativo.StartsWith("http")) return caminhoRelativo;

            return $"{_baseUrl}{caminhoRelativo}";
        }
    }
}
