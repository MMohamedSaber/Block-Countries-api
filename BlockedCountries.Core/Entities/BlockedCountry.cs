
namespace BlockedCountries.Core.Entities
{
    public class BlockedCountry
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
