using System.ComponentModel.DataAnnotations;

namespace buzzaraApi.DTOs
{
    public class NovoUsuarioDTO
    {
        [Required]
        public DateTime DataNascimento { get; set; }
        [Required]
        public string NomeCompleto { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Cpf { get; set; } = null!;
        public string Genero { get; set; } = null!;
        [Required]
        public string Senha { get; set; } = null!;
        [Required]
        public string ConfirmaSenha { get; set; } = null!;
        public bool IsValid { get; set; } = false;

    }
}
