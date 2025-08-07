using System.Security.Claims;
using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("api/boost")]
    [Authorize]
    public class BoostController : ControllerBase
    {
        private readonly BoostService _svc;

        public BoostController(BoostService svc)
            => _svc = svc;

        // GET api/boost/plans
        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
            => Ok(await _svc.ListarPlanosAsync());

        // POST api/boost/order
        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateBoostOrderDto dto)
        {
            var userId = int.Parse(User.FindFirstValue("nameid"));
            var userEmail = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await _svc.CriarPedidoAsync(dto, userId, userEmail);
            return Ok(result);
        }
    }
}
