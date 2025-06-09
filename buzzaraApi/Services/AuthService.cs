using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace buzzaraApi.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthUserDataDTO?> GetUserFromTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return null;

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioID == userId);
                if (usuario == null)
                    return null;

                var abilityRules = GerarAbilityRules(usuario.Role)
                    .Select(rule => new AbilityRuleDTO
                    {
                        Action = (string)rule.GetType().GetProperty("action")!.GetValue(rule)!,
                        Subject = (string)rule.GetType().GetProperty("subject")!.GetValue(rule)!
                    })
                    .ToList();

                return new AuthUserDataDTO
                {
                    Id = usuario.UsuarioID,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role ?? "Acompanhante",
                    Ativo = usuario.Ativo,
                    AbilityRules = abilityRules
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<Usuario?> AutenticarUsuario(LoginDTO loginDTO)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (usuario == null ||
                !BCrypt.Net.BCrypt.Verify(loginDTO.Password, usuario.SenhaHash) ||
                !usuario.IsValid)
                return null;

            return usuario;
        }

        public string GerarAccessToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

            var claims = new List<Claim>
            {
                new Claim("nameid", usuario.UsuarioID.ToString()),
                new Claim("name", usuario.Nome),
                new Claim("email", usuario.Email),
                new Claim("role", usuario.Role ?? "Acompanhante")
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GerarRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task SalvarRefreshToken(Usuario usuario, string refreshToken)
        {
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
        }

        public List<object> GerarAbilityRules(string? role)
        {
            return (role ?? "").ToLower() switch
            {
                "admin" => new List<object>
                {
                    new { action = "manage", subject = "AdminPanel" },
                    new { action = "read",   subject = "Dashboard"  },
                    new { action = "create", subject = "User"       },
                    new { action = "delete", subject = "User"       },
                },
                "acompanhante" => new List<object>
                {
                    new { action = "read",   subject = "Dashboard" },
                    new { action = "update", subject = "Profile"   },
                },
                _ => new List<object>()
            };
        }

        public async Task<AuthResponseDTO?> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenDTO.RefreshToken);

            if (usuario == null || usuario.RefreshTokenExpiration < DateTime.UtcNow)
                return null;

            var newToken = GerarAccessToken(usuario);
            var newRefreshToken = GerarRefreshToken();

            usuario.RefreshToken = newRefreshToken;
            usuario.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = newToken,
                RefreshToken = newRefreshToken,
                UserData = new AuthUserDataDTO
                {
                    Id = usuario.UsuarioID,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role ?? string.Empty,
                    Ativo = usuario.Ativo,
                    AbilityRules = GerarAbilityRules(usuario.Role)
                        .Select(rule => new AbilityRuleDTO
                        {
                            Action = (string)rule.GetType().GetProperty("action")!.GetValue(rule)!,
                            Subject = (string)rule.GetType().GetProperty("subject")!.GetValue(rule)!
                        })
                        .ToList()
                }
            };
        }

        public async Task<bool> EnviarRecuperacaoSenha(RecuperarSenhaDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null) return false;

            var token = Guid.NewGuid().ToString();
            usuario.RefreshToken = token;
            usuario.RefreshTokenExpiration = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RedefinirSenha(RedefinirSenhaDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.Token);

            if (usuario == null || usuario.RefreshTokenExpiration < DateTime.UtcNow)
                return false;

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiration = null;
            await _context.SaveChangesAsync();
            return true;
        }

        // ───────────────────────────────────────────────────────────────────
        // Método para troca de senha do usuário logado:
        // ───────────────────────────────────────────────────────────────────
        public async Task<bool> AlterarSenhaAsync(int usuarioId, AlterarSenhaDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioID == usuarioId);
            if (usuario == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash))
                return false;

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            await _context.SaveChangesAsync();
            return true;
        }

        // ───────────────────────────────────────────────────────────────────
        // MÉTODO ADICIONADO: Obter usuário por ID (usado no logout)
        // ───────────────────────────────────────────────────────────────────
        public async Task<Usuario?> ObterUsuarioPorId(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // ───────────────────────────────────────────────────────────────────
        //  MÉTODO ADICIONADO: Atualizar dados do usuário (status online, IP, etc.)
        // ───────────────────────────────────────────────────────────────────
        public async Task AtualizarUsuario(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
