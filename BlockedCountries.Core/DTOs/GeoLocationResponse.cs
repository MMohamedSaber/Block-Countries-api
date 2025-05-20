
using System.Text.Json.Serialization;

namespace BlockedCountries.Core.DTOs
{
    public class GeoLocationResponse
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("country_code2")]
        public string CountryCode { get; set; }
        
        [JsonPropertyName("country_name")]
        public string country_name { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
        public string isp { get; set; }

        public bool IsBlocked { get; set; }=false;  
    }
}
