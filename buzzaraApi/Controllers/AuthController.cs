using buzzaraApi.DTOs;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace buzzaraApi.Controllers
{
    [Route("/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IWebHostEnvironment _env;

        public AuthController(AuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        private CookieOptions GetCookieOptions(TimeSpan expires)
        {
            var isProduction = _env.IsProduction();

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.Add(expires)
            };
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var claims = User.Claims;
            var userData = new
            {
                Nome = claims.FirstOrDefault(c => c.Type == "name")?.Value,
                Email = claims.FirstOrDefault(c => c.Type == "email")?.Value,
                Role = claims.FirstOrDefault(c => c.Type == "role")?.Value,
                Id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            };
            return Ok(new { userData });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var usuario = await _authService.AutenticarUsuario(loginDTO);
            if (usuario == null)
                return Unauthorized(new { message = "Email ou senha inválidos." });

            // CAPTURA O IP FORÇANDO IPv4
            var ip = HttpContext.Connection.RemoteIpAddress;
            if (ip != null && ip.IsIPv4MappedToIPv6)
                ip = ip.MapToIPv4();

            usuario.EstaOnline = true;
            usuario.UltimoAcesso = DateTime.Now;
            usuario.UltimoIP = ip?.ToString();

            await _authService.AtualizarUsuario(usuario);

            var accessToken = _authService.GerarAccessToken(usuario);
            var refreshToken = _authService.GerarRefreshToken();
            await _authService.SalvarRefreshToken(usuario, refreshToken);

            Response.Cookies.Append("accessToken", accessToken, GetCookieOptions(TimeSpan.FromHours(1)));
            Response.Cookies.Append("refreshToken", refreshToken, GetCookieOptions(TimeSpan.FromDays(7)));

            var abilityRules = usuario.Role?.ToLower() switch
            {
                "admin" => new[]
                {
            new AbilityRuleDTO{ Action="manage", Subject="AdminPanel" },
            new AbilityRuleDTO{ Action="read",   Subject="Dashboard"  },
            new AbilityRuleDTO{ Action="create", Subject="User"       },
            new AbilityRuleDTO{ Action="delete", Subject="User"       },
        },
                "acompanhante" => new[]
                {
            new AbilityRuleDTO{ Action="read",   Subject="Dashboard"  },
            new AbilityRuleDTO{ Action="update", Subject="Profile"    },
        },
                _ => Array.Empty<AbilityRuleDTO>()
            };

            return Ok(new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserData = new AuthUserDataDTO
                {
                    Id = usuario.UsuarioID,
                    Nome = usuario.Nome,
                    Genero = usuario.Genero ?? "Não informado",
                    Email = usuario.Email,
                    Role = usuario.Role!,
                    Ativo = usuario.Ativo,
                    AbilityRules = abilityRules
                }
            });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            var response = await _authService.RefreshToken(refreshTokenDTO);
            if (response == null)
                return Unauthorized(new { message = "Refresh token inválido ou expirado." });

            Response.Cookies.Append("accessToken", response.AccessToken, GetCookieOptions(TimeSpan.FromHours(1)));
            Response.Cookies.Append("refreshToken", response.RefreshToken, GetCookieOptions(TimeSpan.FromDays(7)));

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
            {
                var usuario = await _authService.ObterUsuarioPorId(userId);
                if (usuario != null)
                {
                    // CAPTURA O IP FORÇANDO IPv4
                    var ip = HttpContext.Connection.RemoteIpAddress;
                    if (ip != null && ip.IsIPv4MappedToIPv6)
                        ip = ip.MapToIPv4();

                    usuario.EstaOnline = false;
                    usuario.UltimoAcesso = DateTime.Now;
                    usuario.UltimoIP = ip?.ToString();

                    await _authService.AtualizarUsuario(usuario);
                }
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logout realizado com sucesso." });
        }


        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaDTO dto)
        {
            var sucesso = await _authService.EnviarRecuperacaoSenha(dto);
            if (!sucesso)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(new { message = "Um e-mail de recuperação foi enviado." });
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDTO dto)
        {
            var sucesso = await _authService.RedefinirSenha(dto);
            if (!sucesso)
                return BadRequest(new { message = "Token inválido ou expirado." });

            return Ok(new { message = "Senha redefinida com sucesso!" });
        }

        [HttpPost("alterar-senha")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToArray();
                return BadRequest(new { errors });
            }

            var idClaim = User.Claims
                              .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid")
                              ?.Value;
            if (!int.TryParse(idClaim, out var usuarioId))
                return Unauthorized(new { message = "Usuário não identificado." });

            var sucesso = await _authService.AlterarSenhaAsync(usuarioId, dto);
            if (!sucesso)
                return BadRequest(new { message = "A senha atual está incorreta." });

            return Ok(new { message = "Senha alterada com sucesso!" });
        }
    }
}
