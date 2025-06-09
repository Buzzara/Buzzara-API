namespace buzzaraApi.DTOs
{
    public class PerfilUsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Genero { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string? UltimoIP { get; set; }
        public bool EstaOnline { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string? FotoCapaUrl { get; set; }

        // Novos campos
        public string? Descricao { get; set; }
        public string? Localizacao { get; set; }

    }
}
