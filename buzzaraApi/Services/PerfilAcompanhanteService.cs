using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Services
{
    public class PerfilAcompanhanteService
    {
        private readonly ApplicationDbContext _context;

        public PerfilAcompanhanteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo perfil de acompanhante
        /// </summary>
        /// <param name="dto">Dados para criação do perfil</param>
        /// <returns>PerfilAcompanhante criado</returns>
        public async Task<PerfilAcompanhante> CriarPerfil(PerfilAcompanhanteDTO dto)
        {
            // Verifica se já existe um perfil para esse usuário
            var existe = await _context.PerfisAcompanhantes
                .AnyAsync(p => p.UsuarioID == dto.UsuarioID);

            if (existe)
                throw new InvalidOperationException("Este usuário já possui um perfil.");

            var perfil = new PerfilAcompanhante
            {
                UsuarioID = dto.UsuarioID,
                Descricao = dto.Descricao,
                Localizacao = dto.Localizacao,
                Tarifa = dto.Tarifa,
                DataAtualizacao = DateTime.Now
            };

            _context.PerfisAcompanhantes.Add(perfil);
            await _context.SaveChangesAsync();
            return perfil;
        }

        /// <summary>
        /// Buscar um perfil de acompanhante por ID
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <returns>PerfilAcompanhante ou null se não encontrado</returns>
        public async Task<UsuarioPerfilDTO?> ObterUsuarioPerfilPorId(int id)
        {
            var perfil = await _context.PerfisAcompanhantes
                .Include(p => p.Usuario)
                .Include(p => p.Fotos)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.PerfilAcompanhanteID == id);

            if (perfil == null || perfil.Usuario == null)
                return null;

            return new UsuarioPerfilDTO
            {
                UsuarioID = perfil.Usuario.UsuarioID,
                PerfilAcompanhanteID = perfil.PerfilAcompanhanteID,
                Nome = perfil.Usuario.Nome,
                Email = perfil.Usuario.Email,
                Telefone = perfil.Usuario.Telefone,
                Genero = perfil.Usuario.Genero,
                DataNascimento = perfil.Usuario.DataNascimento,
                FotoPerfilUrl = perfil.Usuario.FotoPerfilUrl,
                FotoCapaUrl = perfil.Usuario.FotoCapaUrl,
                Descricao = perfil.Descricao,
                EstaOnline = perfil.Usuario.EstaOnline,
                UltimoAcesso = perfil.Usuario.UltimoAcesso,
                UltimoIP = perfil.Usuario.UltimoIP,
                Localizacao = perfil.Localizacao,
                Tarifa = perfil.Tarifa
            };
        }

        /// <summary>
        /// Atualizar um perfil de acompanhante
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <param name="dto">Dados atualizados</param>
        /// <returns>Perfil atualizado ou null se não encontrado</returns>
        public async Task<PerfilAcompanhante?> AtualizarPerfil(int id, PerfilAcompanhanteDTO dto)
        {
            var perfil = await _context.PerfisAcompanhantes
                .Include(p => p.Usuario) // Inclui o usuário para poder editar o telefone
                .FirstOrDefaultAsync(p => p.PerfilAcompanhanteID == id);

            if (perfil == null)
                return null;

            perfil.Descricao = dto.Descricao;
            perfil.Localizacao = dto.Localizacao;
            perfil.Tarifa = dto.Tarifa;
            perfil.DataAtualizacao = DateTime.Now;

            // Atualiza telefone do usuário, se fornecido
            if (!string.IsNullOrWhiteSpace(dto.Telefone))
            {
                perfil.Usuario.Telefone = dto.Telefone;
            }

            await _context.SaveChangesAsync();
            return perfil;
        }

        /// <summary>
        /// Deletar um perfil de acompanhante
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <returns>true se deletou, false se não encontrado</returns>
        public async Task<bool> DeletarPerfil(int id)
        {
            var perfil = await _context.PerfisAcompanhantes.FindAsync(id);
            if (perfil == null)
                return false;

            _context.PerfisAcompanhantes.Remove(perfil);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Listar todos os perfis (sem paginação)
        /// </summary>
        /// <returns>Lista de PerfilAcompanhante</returns>
        public async Task<List<UsuarioPerfilDTO>> ObterTodosPerfisComUsuario()
        {
            return await _context.PerfisAcompanhantes
                .Include(p => p.Usuario)
                .Include(p => p.Fotos)
                .Include(p => p.Videos)
                .Select(p => new UsuarioPerfilDTO
                {
                    UsuarioID = p.Usuario.UsuarioID,
                    PerfilAcompanhanteID = p.PerfilAcompanhanteID,
                    Nome = p.Usuario.Nome,
                    Email = p.Usuario.Email,
                    Telefone = p.Usuario.Telefone,
                    Genero = p.Usuario.Genero,
                    DataNascimento = p.Usuario.DataNascimento,
                    FotoPerfilUrl = p.Usuario.FotoPerfilUrl,
                    FotoCapaUrl = p.Usuario.FotoCapaUrl,
                    Descricao = p.Descricao,
                    EstaOnline = p.Usuario.EstaOnline,
                    UltimoAcesso = p.Usuario.UltimoAcesso,
                    UltimoIP = p.Usuario.UltimoIP,
                    Localizacao = p.Localizacao,
                    Tarifa = p.Tarifa
                })
                .ToListAsync();
        }

        /// <summary>
        /// Listar todos os perfis com paginação
        /// </summary>
        /// <param name="pageNumber">Página atual</param>
        /// <param name="pageSize">Quantidade de registros por página</param>
        /// <returns>Lista de PerfilAcompanhante</returns>
        public async Task<List<PerfilAcompanhante>> ObterTodosPerfis(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.PerfisAcompanhantes
                .Include(p => p.Fotos)
                .Include(p => p.Videos)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Buscar perfis por localização e faixa de tarifa
        /// </summary>
        /// <param name="localizacao">Texto a ser buscado em Localizacao</param>
        /// <param name="tarifaMin">Tarifa mínima</param>
        /// <param name="tarifaMax">Tarifa máxima</param>
        /// <returns>Lista de PerfilAcompanhante filtrada</returns>
        public async Task<List<PerfilAcompanhante>> BuscarPerfis(string? localizacao, decimal? tarifaMin, decimal? tarifaMax)
        {
            var query = _context.PerfisAcompanhantes.AsQueryable();

            if (!string.IsNullOrEmpty(localizacao))
                query = query.Where(p => p.Localizacao != null && p.Localizacao.Contains(localizacao));

            if (tarifaMin.HasValue)
                query = query.Where(p => p.Tarifa >= tarifaMin.Value);

            if (tarifaMax.HasValue)
                query = query.Where(p => p.Tarifa <= tarifaMax.Value);

            return await query
                .Include(p => p.Fotos)
                .Include(p => p.Videos)
                .ToListAsync();
        }
        public async Task<bool> ExistePerfilPorUsuarioId(int usuarioId)
        {
            return await _context.PerfisAcompanhantes.AnyAsync(p => p.UsuarioID == usuarioId);
        }

        public async Task<Usuario?> ObterUsuarioPorId(int usuarioId)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioID == usuarioId);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
