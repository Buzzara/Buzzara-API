using System.ComponentModel.DataAnnotations;

namespace buzzaraApi.DTOs
{
    public class AlterarSenhaDTO
    {
        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha atual deve ter ao menos 6 caracteres.")]
        public string SenhaAtual { get; set; } = null!;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter ao menos 6 caracteres.")]
        public string NovaSenha { get; set; } = null!;

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
        [Compare("NovaSenha", ErrorMessage = "A confirmação não confere com a nova senha.")]
        public string ConfirmarNovaSenha { get; set; } = null!;
    }
}
