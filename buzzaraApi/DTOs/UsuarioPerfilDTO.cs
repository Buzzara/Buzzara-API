namespace buzzaraApi.DTOs
{
    public class UsuarioPerfilDTO
    {
        public int UsuarioID { get; set; }
        public int PerfilAcompanhanteID { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefone { get; set; }
        public string? Genero { get; set; }
        public DateTime DataNascimento { get; set; }

        public string? FotoPerfilUrl { get; set; }
        public string? FotoCapaUrl { get; set; }

        // Informações do perfil acompanhante (opcional)
        public string? Descricao { get; set; }
        public string? Localizacao { get; set; }
        public decimal Tarifa { get; set; }
        public bool EstaOnline { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string? UltimoIP { get; set; }

    }
}
