using Microsoft.EntityFrameworkCore;

namespace buzzaraApi.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string SenhaHash { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public bool IsValid { get; set; } = false;
        public required string Telefone { get; set; }
        public required string Cpf { get; set; }
        public string? Genero { get; set; }
        public string? ValidationToken { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime? ValidationTokenExpiration { get; set; }
        public bool Ativo { get; set; } = true;
        public string? Role { get; set; } = "Acompanhante";
        public string? FotoPerfilUrl { get; set; }
        public string? FotoCapaUrl { get; set; }
        public ICollection<PerfilAcompanhante> PerfisAcompanhantes { get; set; } = new List<PerfilAcompanhante>();
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public bool EstaOnline { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string? UltimoIP { get; set; }


    }
}
