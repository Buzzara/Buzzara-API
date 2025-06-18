namespace buzzaraApi.Models
{
    public class SobreUsuario
    {
        public int Id { get; set; }

        public List<string> Atendimento { get; set; } = new List<string>();  // Exemplo: ["Homens", "Casais"]
        public string? Etnia { get; set; }
        public string? Relacionamento { get; set; }      // Exemplo: "Ativo", "Passivo"
        public string? Cabelo { get; set; }
        public string? Estatura { get; set; }
        public string? Corpo { get; set; }
        public string? Seios { get; set; }
        public string? Pubis { get; set; }

        // FK de relacionamento com o Serviço (Anúncio)
        public int ServicoId { get; set; }
        public Servico Servico { get; set; } = null!;
    }
}
