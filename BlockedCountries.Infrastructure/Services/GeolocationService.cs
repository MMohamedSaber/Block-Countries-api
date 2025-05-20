
using System.Net;
using System.Text.Json;
using BlockedCountries.Core;
using BlockedCountries.Core.interfaces;
using Microsoft.Extensions.Configuration    ;
using Microsoft.Extensions.Logging;

namespace BlockedCountries.Infrastructure.Services
{
    public class GeolocationService :IGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<GeolocationService> _logger;

        public GeolocationService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<GeolocationService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_config["GeoLocationApi:BaseUrl"]);
            _httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(_config["GeoLocationApi:TimeoutSeconds"]));
        }

        public async Task<GeoLocationResponse> GetGeoDataAsync(string ipAddress)
        {
            try
            {
                var apiKey = _config["GeoLocationApi:ApiKey"];
                
                var response = await _httpClient.GetAsync($"?apiKey={apiKey}&ip={ipAddress}");

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Rate limit exceeded while calling GeoLocation API for IP: {Ip}", ipAddress);
                    return GetFallbackLocation(ipAddress);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("GeoLocation API returned non-success status {StatusCode} for IP: {Ip}",
                        response.StatusCode, ipAddress);
                    return GetFallbackLocation(ipAddress);
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Geo API JSON json: " + json);
                var result = JsonSerializer.Deserialize<GeoLocationResponse>(json);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching geo data for IP: {Ip}", ipAddress);
                throw;
            }
        }
        private GeoLocationResponse GetFallbackLocation(string ipAddress)
        {
            return new GeoLocationResponse
            {
                CountryCode = "XX", // Unknown or default
                country_name = "Unknown",
                Ip = ipAddress
            };
        }


    }
}
