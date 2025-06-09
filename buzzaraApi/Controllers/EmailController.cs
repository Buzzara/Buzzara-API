using buzzaraApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buzzaraApi.Controllers
{
    [ApiController]
    [Route("email")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("new-user")]
        public async Task<IActionResult> EnviarTeste([FromQuery] string para)
        {
            string assunto = "Confirmação de Conta - Buzzara";
            string corpo = "<h1>Bem-vindo à Buzzara!</h1><p>Clique no botão abaixo para confirmar sua conta.</p>";

            await _emailService.EnviarEmailAsync(para, assunto, corpo);

            return Ok("Email enviado com sucesso.");
        }
    }
}
