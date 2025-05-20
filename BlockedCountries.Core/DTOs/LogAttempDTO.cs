
namespace BlockedCountries.Core.DTOs
{
    public class LogAttempDTO
    {
        public string IpAddress { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool IsBlocked { get; set; }=true;
        public string userAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Message => IsBlocked == true ? "Access from this country is blocked." : "Access allowed.";
    }
}
