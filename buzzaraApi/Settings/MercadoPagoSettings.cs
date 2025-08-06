namespace buzzaraApi.Settings
{
    public class MercadoPagoSettings
    {
        public string AccessToken { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string SuccessUrl { get; set; } = null!;
        public string FailureUrl { get; set; } = null!;
        public string PendingUrl { get; set; } = null!;
    }
}
