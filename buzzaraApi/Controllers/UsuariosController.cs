using buzzaraApi.DTOs;
using buzzaraApi.Models;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace buzzaraApi.Controllers
{
    [Route("/usuarios")]
    [ApiController]
    [Authorize(Roles = "Acompanhante,Admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                var novoUsuario = await _usuarioService.RegistrarUsuario(usuario);
                return Ok(new { message = "Usuário cadastrado com sucesso!", usuario = novoUsuario });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.ObterTodosUsuarios();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObterUsuarioPorId(id);
                if (usuario == null)
                    return NotFound(new { message = "Usuário não encontrado!" });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> ObterMeuPerfil()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue("nameid")!);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var perfil = await _usuarioService.ObterPerfilDoUsuario(userId, baseUrl);

                if (perfil == null)
                    return NotFound(new { message = "Usuário não encontrado." });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] Usuario usuarioAtualizado)
        {
            try
            {
                var usuario = await _usuarioService.AtualizarUsuario(id, usuarioAtualizado);
                if (usuario == null)
                    return NotFound(new { message = "Usuário não encontrado!" });

                return Ok(new { message = "Usuário atualizado com sucesso!", usuario });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            try
            {
                var sucesso = await _usuarioService.DeletarUsuario(id);
                if (!sucesso)
                    return NotFound(new { message = "Usuário não encontrado!" });

                return Ok(new { message = "Usuário deletado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload-fotos")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFotos([FromForm] UploadFotoPerfilDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue("nameid")!);
                await _usuarioService.AtualizarFotosPerfilAsync(userId, dto.FotoPerfil, dto.FotoCapa);
                return Ok(new { message = "Fotos atualizadas com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("remover-foto/{tipo}")]
        [Authorize]
        public async Task<IActionResult> RemoverFoto(string tipo) // tipo: perfil ou capa
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue("nameid")!);
                await _usuarioService.RemoverFotoAsync(userId, tipo.ToLower());
                return Ok(new { message = $"Foto de {tipo} removida com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
