namespace buzzaraApi.Models
{
    public class PerfilAcompanhante
    {
        public int PerfilAcompanhanteID { get; set; }

        // FK para usuário
        public int UsuarioID { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string? Descricao { get; set; }
        public string? Localizacao { get; set; }
        public decimal Tarifa { get; set; }
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;

        // Relações:
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<FotoAcompanhante> Fotos { get; set; } = new List<FotoAcompanhante>();
        public ICollection<VideoAcompanhante> Videos { get; set; } = new List<VideoAcompanhante>();
        public ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
