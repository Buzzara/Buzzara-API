using buzzaraApi.DTOs;
using buzzaraApi.Models;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.Controllers
{
    [Route("/perfis")]
    [ApiController]
    [Authorize]
    public class PerfisAcompanhantesController : ControllerBase
    {
        private readonly PerfilAcompanhanteService _perfilService;

        public PerfisAcompanhantesController(PerfilAcompanhanteService perfilService)
        {
            _perfilService = perfilService;
        }

        // POST: api/perfis
        [HttpPost]
        public async Task<IActionResult> CriarPerfil([FromBody] PerfilAcompanhanteDTO dto)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst("nameid")?.Value ?? "0");
                if (usuarioId == 0)
                    return Unauthorized();

                dto.UsuarioID = usuarioId;

                var jaExiste = await _perfilService.ExistePerfilPorUsuarioId(usuarioId);
                if (jaExiste)
                    return BadRequest(new { message = "Perfil já criado anteriormente." });

                var perfil = await _perfilService.CriarPerfil(dto);

                // Ativa o usuário
                var usuario = await _perfilService.ObterUsuarioPorId(usuarioId);
                if (usuario != null)
                {
                    usuario.Telefone = dto.Telefone;
                    usuario.Ativo = true;
                    usuario.UltimoAcesso = DateTime.Now;
                    await _perfilService.SalvarAlteracoesAsync();
                }

                return Ok(new { message = "Perfil criado e usuário ativado com sucesso!", perfilId = perfil.PerfilAcompanhanteID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/perfis/all
        [HttpGet("all")]
        public async Task<IActionResult> ObterTodosPerfis()
        {
            var perfis = await _perfilService.ObterTodosPerfisComUsuario();
            return Ok(perfis);
        }

        // GET: api/perfis/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPerfilPorId(int id)
        {
            var perfil = await _perfilService.ObterUsuarioPerfilPorId(id);
            if (perfil == null)
                return NotFound(new { message = "Perfil não encontrado!" });

            return Ok(perfil);
        }

        // PUT: api/perfis/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPerfil(int id, [FromBody] PerfilAcompanhanteDTO dto)
        {
            var perfil = await _perfilService.AtualizarPerfil(id, dto);
            if (perfil == null)
                return NotFound(new { message = "Perfil não encontrado!" });

            return Ok(perfil);
        }

        // DELETE: api/perfis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPerfil(int id)
        {
            var sucesso = await _perfilService.DeletarPerfil(id);
            if (!sucesso)
                return NotFound(new { message = "Perfil não encontrado!" });

            return Ok(new { message = "Perfil deletado com sucesso!" });
        }
    }
}
