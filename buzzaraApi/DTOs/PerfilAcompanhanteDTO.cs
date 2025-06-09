namespace buzzaraApi.DTOs
{
    public class PerfilAcompanhanteDTO
    {
        public int PerfilAcompanhanteID { get; set; }
        public int UsuarioID { get; set; }
        public string? Descricao { get; set; }
        public string? Localizacao { get; set; }
        public decimal Tarifa { get; set; }
        public string Telefone { get; set; }
        public bool EstaOnline { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public string? UltimoIP { get; set; }

    }
}
