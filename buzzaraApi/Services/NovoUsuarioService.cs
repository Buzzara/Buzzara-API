using buzzaraApi.Data;
using buzzaraApi.DTOs;
using buzzaraApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace buzzaraApi.Services
{
    public class NovoUsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public NovoUsuarioService(ApplicationDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<Usuario?> RegistrarNovoUsuario(NovoUsuarioDTO dto)
        {
            if (dto.Senha != dto.ConfirmaSenha)
                return null;

            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return null;

            string email = dto.Email?.ToLower() ?? "";
            string role = email.EndsWith("@buzzara.com.br") ? "admin" : "Acompanhante";

            var user = new Usuario
            {
                Nome = dto.NomeCompleto,
                Email = dto.Email ?? "",
                Telefone = dto.Telefone,
                DataNascimento = dto.DataNascimento,
                Cpf = dto.Cpf,
                Genero = dto.Genero,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                DataCadastro = DateTime.Now,
                Role = role,
                IsValid = false,
                Ativo = false,
                ValidationToken = GenerateValidationToken(),
                ValidationTokenExpiration = DateTime.UtcNow.AddHours(24)
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            await SendValidationEmail(user);
            return user;
        }

        public async Task<Usuario?> ValidateUser(string token)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.ValidationToken == token && u.ValidationTokenExpiration > DateTime.UtcNow);

            if (user == null)
                return null;

            user.IsValid = true;
            user.ValidationToken = null;
            user.ValidationTokenExpiration = null;

            await _context.SaveChangesAsync();
            return user;
        }

        private string GenerateValidationToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task SendValidationEmail(Usuario user)
        {
            var backUrl = _configuration["AppSettings:BackendUrl"];
            if (string.IsNullOrWhiteSpace(backUrl))
                throw new InvalidOperationException("AppSettings:BackendUrl não configurado.");

            var confirmationLink = $"{backUrl}/novousuario/validate?token={Uri.EscapeDataString(user.ValidationToken!)}";

            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto;'>
                    <h2 style='color: #007bff;'>Confirmação de Conta - Buzzara</h2>
                    <p>Olá {user.Nome},</p>
                    <p>Obrigado por se cadastrar na Buzzara!</p>
                    <p>Para ativar sua conta, clique no botão abaixo:</p>
                    <p style='text-align: center;'>
                        <a href='{confirmationLink}' style='
                            display: inline-block;
                            padding: 12px 24px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;'>Validar Conta</a>
                    </p>
                    <p>Este link expirará em 24 horas.</p>
                    <hr />
                    <p style='font-size: 12px; color: #999;'>Se você não solicitou este cadastro, ignore este e-mail.</p>
                </div>
            ";

            await _emailService.EnviarEmailAsync(user.Email, "Confirmação de Conta - Buzzara", emailBody);
        }
    }
}
