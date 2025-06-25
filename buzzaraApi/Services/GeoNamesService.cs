using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using buzzaraApi.DTOs;
using Microsoft.Extensions.Configuration;

namespace buzzaraApi.Services
{
    public class GeoNamesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GeoNamesService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<LocalidadeProximaDTO>> ObterLocalidadesProximasAsync(double latitude, double longitude)
        {
            var username = _config["GeoNames:Username"] ?? "buzzara"; // configurar no appsettings ou docker.env
            var url = $"https://secure.geonames.org/findNearbyPlaceNameJSON?lat={latitude}&lng={longitude}&radius=10&username={username}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<GeoNamesResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (resultado?.Geonames == null)
                return new List<LocalidadeProximaDTO>();

            return resultado.Geonames
                .Select(g => new LocalidadeProximaDTO
                {
                    Nome = g.Name,
                    Cidade = g.AdminName1,
                    Estado = g.AdminCodes1?.ISO3166_2 ?? string.Empty,
                    DistanciaKm = double.TryParse(g.Distance, out var d) ? d : 0
                })
                .OrderBy(l => l.DistanciaKm)
                .ToList();

        }
    }

    // Classes auxiliares para deserialização
    public class GeoNamesResponse
    {
        public List<GeoNameItem>? Geonames { get; set; }
    }

    public class GeoNameItem
    {
        public string? Name { get; set; }

        [JsonPropertyName("adminName1")]
        public string? AdminName1 { get; set; }

        [JsonPropertyName("adminCodes1")]
        public AdminCodes? AdminCodes1 { get; set; }

        [JsonPropertyName("distance")]
        public string? Distance { get; set; }
    }


    public class AdminCodes
    {
        public string? ISO3166_2 { get; set; } // Ex: SP
    }
}
