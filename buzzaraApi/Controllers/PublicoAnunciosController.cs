using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("publico/anuncios")]
[Produces("application/json")]    // força o Swagger a usar JSON
public class PublicoAnunciosController : ControllerBase
{
    private readonly AnuncioPublicoService _service;
    public PublicoAnunciosController(AnuncioPublicoService service)
        => _service = service;

    // GET /publico/anuncios
    [HttpGet]
    public async Task<IActionResult> GetTodos(
        [FromQuery] string? cidade,
        [FromQuery] string? estado,
        [FromQuery] string? bairro)
    {
        var anuncios = await _service.ListarTodos(cidade, estado, bairro);
        return Ok(anuncios);
    }

    // GET /publico/anuncios/usuario/5
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<IActionResult> GetPorUsuario(int usuarioId)
    {
        var anuncios = await _service.ListarPorUsuario(usuarioId);
        return Ok(anuncios);
    }

    // GET /publico/anuncios/123
    [HttpGet("{servicoId:int}")]
    [ProducesResponseType(typeof(AnuncioPublicoDTO), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AnuncioPublicoDTO>> GetAnuncio(int servicoId)
    {
        var dto = await _service.ObterPorIdAsync(servicoId);
        if (dto == null)
            return NotFound(new { mensagem = "Anúncio não encontrado." });

        return Ok(dto);
    }
}
