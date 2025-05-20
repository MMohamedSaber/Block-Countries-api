
namespace BlockedCountries.Core.Models
{
    public class TemporalBlock
    {
        public string CountryCode { get; set; }
        public DateTime BlockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
