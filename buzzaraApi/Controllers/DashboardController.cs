using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using buzzaraApi.DTOs;
using buzzaraApi.Services;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
            => _dashboardService = dashboardService;

        [HttpGet]
        public ActionResult<DashboardDataDTO> Get()
        {
            // 1) Garantir que o token foi validado
            if (!User.Identity?.IsAuthenticated ?? false)
                return Unauthorized();

            // 2) Pegar o claim e converter para int
            var raw = User.FindFirstValue("nameid");
            if (!int.TryParse(raw, out var userId))
                return Unauthorized(new { message = "Token inválido ou expirado." });

            // 3) Chamar o service
            var dto = _dashboardService.ObterMetricasUsuario(userId);
            return Ok(dto);
        }
    }
}
