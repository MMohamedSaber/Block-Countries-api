
using System.ComponentModel.DataAnnotations;

namespace BlockedCountries.Core.DTOs
{
    public class TemporalBlockDTO
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }  
      
        
        [Range(1, 1440)]
        public int DurationMinutes { get; set; }
    }
}
