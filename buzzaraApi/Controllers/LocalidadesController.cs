using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("localidades")]
    public class LocalidadesController : ControllerBase
    {
        private readonly GeoNamesService _geoService;

        public LocalidadesController(GeoNamesService geoService)
        {
            _geoService = geoService;
        }

        [HttpGet("localidades-proximas")]
        public async Task<IActionResult> GetLocalidadesProximas([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var resultados = await _geoService.ObterLocalidadesProximasAsync(latitude, longitude);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar localidades: {ex.Message}");
            }
        }
    }
}
