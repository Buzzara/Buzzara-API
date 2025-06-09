using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("publico")]
    public class PublicoController : ControllerBase
    {
        private readonly PublicoService _publicoService;

        public PublicoController(PublicoService publicoService)
        {
            _publicoService = publicoService;
        }

        /// <summary>
        /// Retorna o perfil público de um acompanhante.
        /// </summary>
        [HttpGet("perfil/{usuarioId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPerfilPublico(int usuarioId)
        {
            var perfil = await _publicoService.ObterPerfilPublico(usuarioId);

            if (perfil == null)
                return NotFound(new { message = "Perfil não encontrado." });

            return Ok(perfil);
        }
    }
}
