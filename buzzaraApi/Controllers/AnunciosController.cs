using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("anuncios")]
    [Authorize(Roles = "Acompanhante,Admin")]
    public class AnunciosController : ControllerBase
    {
        private readonly ServicoService _svc;

        public AnunciosController(ServicoService svc) => _svc = svc;

        #region Helpers

        private int GetUserId()
        {
            var rawId = User.FindFirstValue("nameid");
            if (string.IsNullOrEmpty(rawId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return int.Parse(rawId);
        }

        #endregion

        #region CRUD Anúncios

        /// <summary>
        /// Cria um novo anúncio com até 4 fotos e 1 vídeo de apresentação (até 50s).
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateServicoDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var created = await _svc.CreateAsync(dto, userId);
                return Ok(created);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os anúncios do usuário autenticado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var userId = GetUserId();
                var list = await _svc.GetAllByUserAsync(userId);
                return Ok(list);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        #endregion

        #region Upload Fotos e Vídeos

        /// <summary>
        /// Faz o upload de uma nova foto para o anúncio especificado.
        /// </summary>
        [HttpPost("{id:int}/fotos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFoto(int id, [FromForm] UploadFotoAnuncioDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var res = await _svc.UploadFotoAsync(id, dto.File, userId);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Faz o upload de um vídeo de apresentação para o anúncio especificado.
        /// </summary>
        [HttpPost("{id:int}/videos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadVideo(int id, [FromForm] UploadVideoAnuncioDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var res = await _svc.UploadVideoAsync(id, dto.File, userId);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Comprar Pacote de Fotos

        /// <summary>
        /// Compra um novo pacote de até 4 fotos adicionais para o anúncio.
        /// </summary>
        [HttpPost("{id:int}/comprar-fotos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ComprarPacoteFotos(int id, [FromForm] List<IFormFile> fotos)
        {
            try
            {
                var userId = GetUserId();

                if (fotos == null || fotos.Count == 0)
                    return BadRequest(new { error = "Nenhuma foto enviada." });

                if (fotos.Count > 4)
                    return BadRequest(new { error = "Máximo de 4 fotos por pacote." });

                foreach (var foto in fotos)
                {
                    await _svc.UploadFotoAsync(id, foto, userId);
                }

                return Ok(new { message = "Fotos adicionadas com sucesso." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Edita um anúncio existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateServicoDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var updated = await _svc.UpdateAsync(id, userId, dto);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Remove um anúncio existente.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _svc.DeleteAsync(id, userId);
                return success ? Ok() : NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }

}
