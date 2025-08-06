using Microsoft.AspNetCore.Mvc;
using buzzaraApi.DTOs;
using buzzaraApi.Services;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PreferenceService _preferenceService;

        public PaymentController(PreferenceService preferenceService)
            => _preferenceService = preferenceService;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePreferenceDto dto)
        {
            if (dto == null || dto.UnitPrice <= 0)
                return BadRequest(new { error = "Dados inválidos." });

            var pref = await _preferenceService.CreateAsync(dto);
            return Ok(new
            {
                id = pref.Id,
                initPoint = pref.InitPoint,    // link para checkout web
                sandboxInitPoint = pref.SandboxInitPoint
            });
        }

        // Opcional: webhook
        [HttpPost("webhook")]
        public IActionResult Webhook([FromBody] dynamic notification)
        {
            // Trate notification.type e notification.data.id
            return Ok();
        }
    }
}
