using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace buzzaraApi.Controllers
{
    [Route("/novousuario")]
    [ApiController]
    public class NovoUsuarioController : ControllerBase
    {
        private readonly NovoUsuarioService _novoUsuarioService;
        private readonly IConfiguration _configuration;

        public NovoUsuarioController(NovoUsuarioService novoUsuarioService, IConfiguration configuration)
        {
            _novoUsuarioService = novoUsuarioService;
            _configuration = configuration;
        }

        // POST: novousuario/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] NovoUsuarioDTO dto)
        {
            var user = await _novoUsuarioService.RegistrarNovoUsuario(dto);

            if (user == null)
            {
                return BadRequest(new
                {
                    message = "Não foi possível registrar. O e-mail já está em uso ou as senhas não coincidem."
                });
            }

            return Ok(new
            {
                message = "Usuário registrado com sucesso! Verifique seu e-mail para validar a conta."
            });
        }

        // GET: novousuario/validate
        [HttpGet("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "Token de validação não informado." });
            }

            var user = await _novoUsuarioService.ValidateUser(token);
            if (user == null)
            {
                return BadRequest(new { message = "Token inválido ou expirado." });
            }

            // Redireciona dinamicamente para a URL de login definida no appsettings/env
            var frontUrl = _configuration["AppSettings:FrontendUrl"];
            if (string.IsNullOrWhiteSpace(frontUrl))
            {
                return Ok(new { message = "Usuário validado com sucesso. Por favor, acesse sua conta." });
            }

            return Redirect($"{frontUrl}/login");
        }
    }
}
