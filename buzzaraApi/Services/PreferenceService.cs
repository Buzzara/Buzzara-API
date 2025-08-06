using MercadoPago.Client.Preference;
using MercadoPago.Config;              // traz MercadoPagoConfig
using MercadoPago.Resource.Preference; // traz Preference e sub-tipos
using buzzaraApi.DTOs;
using buzzaraApi.Settings;
using Microsoft.Extensions.Options;

namespace buzzaraApi.Services
{
    public class PreferenceService
    {
        private readonly MercadoPagoSettings _settings;

        public PreferenceService(IOptions<MercadoPagoSettings> opts)
        {
            _settings = opts.Value;
        }

        public async Task<Preference> CreateAsync(CreatePreferenceDto dto)
        {
            // o PreferenceClient usa o MercadoPagoConfig.AccessToken que você já setou
            var client = new PreferenceClient();

            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new()
                    {
                        Title     = dto.Title,
                        Quantity  = dto.Quantity,
                        UnitPrice = dto.UnitPrice
                    }
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = _settings.SuccessUrl,
                    Failure = _settings.FailureUrl,
                    Pending = _settings.PendingUrl
                },
                AutoReturn = "approved"
            };

            return await client.CreateAsync(request);
        }
    }
}
