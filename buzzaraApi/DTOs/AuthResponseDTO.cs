namespace buzzaraApi.DTOs
{
    public class AuthResponseDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required AuthUserDataDTO UserData { get; set; }
    }

    public class AuthUserDataDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Genero { get; set; }
        public required bool EstaOnline { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public bool Ativo { get; set; }

        public IEnumerable<AbilityRuleDTO> AbilityRules { get; set; } = new List<AbilityRuleDTO>();
    }

    public class AbilityRuleDTO
    {
        public required string Action { get; set; }
        public required string Subject { get; set; }
    }
}
