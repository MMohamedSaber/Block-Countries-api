
using BlockedCountries.Core.Entities;

namespace BlockedCountries.Infrastructure
{
    public class SeedBlockedCountry
    {

        public SeedBlockedCountry()
        {
           

        }


        public static List<BlockedCountry> init()
        {
            var initialCountries = new List<BlockedCountry>
    {
       new BlockedCountry { CountryCode = "EG", CountryName = "Egypt" },
new BlockedCountry { CountryCode = "US", CountryName = "United States" },
new BlockedCountry { CountryCode = "CA", CountryName = "Canada" },
new BlockedCountry { CountryCode = "GB", CountryName = "United Kingdom" },
new BlockedCountry { CountryCode = "FR", CountryName = "France" },
new BlockedCountry { CountryCode = "DE", CountryName = "Germany" },
new BlockedCountry { CountryCode = "IT", CountryName = "Italy" },
new BlockedCountry { CountryCode = "ES", CountryName = "Spain" },
new BlockedCountry { CountryCode = "RU", CountryName = "Russia" },
new BlockedCountry { CountryCode = "CN", CountryName = "China" },
new BlockedCountry { CountryCode = "JP", CountryName = "Japan" },
new BlockedCountry { CountryCode = "KR", CountryName = "South Korea" },
new BlockedCountry { CountryCode = "BR", CountryName = "Brazil" },
new BlockedCountry { CountryCode = "IN", CountryName = "India" },
new BlockedCountry { CountryCode = "AU", CountryName = "Australia" },
new BlockedCountry { CountryCode = "ZA", CountryName = "South Africa" },
new BlockedCountry { CountryCode = "MX", CountryName = "Mexico" },
new BlockedCountry { CountryCode = "AR", CountryName = "Argentina" },
new BlockedCountry { CountryCode = "TR", CountryName = "Turkey" },
new BlockedCountry { CountryCode = "SA", CountryName = "Saudi Arabia" },
new BlockedCountry { CountryCode = "AE", CountryName = "United Arab Emirates" },
new BlockedCountry { CountryCode = "IR", CountryName = "Iran" },
new BlockedCountry { CountryCode = "IQ", CountryName = "Iraq" },
new BlockedCountry { CountryCode = "SY", CountryName = "Syria" },
new BlockedCountry { CountryCode = "JO", CountryName = "Jordan" },
new BlockedCountry { CountryCode = "LB", CountryName = "Lebanon" },
new BlockedCountry { CountryCode = "SD", CountryName = "Sudan" },
new BlockedCountry { CountryCode = "LY", CountryName = "Libya" },
new BlockedCountry { CountryCode = "TN", CountryName = "Tunisia" },
new BlockedCountry { CountryCode = "DZ", CountryName = "Algeria" },
new BlockedCountry { CountryCode = "MA", CountryName = "Morocco" },
new BlockedCountry { CountryCode = "PK", CountryName = "Pakistan" },
new BlockedCountry { CountryCode = "BD", CountryName = "Bangladesh" },
new BlockedCountry { CountryCode = "ID", CountryName = "Indonesia" },
new BlockedCountry { CountryCode = "TH", CountryName = "Thailand" },
new BlockedCountry { CountryCode = "VN", CountryName = "Vietnam" },
new BlockedCountry { CountryCode = "PH", CountryName = "Philippines" },
new BlockedCountry { CountryCode = "MY", CountryName = "Malaysia" },
new BlockedCountry { CountryCode = "SG", CountryName = "Singapore" },
new BlockedCountry { CountryCode = "NG", CountryName = "Nigeria" },
new BlockedCountry { CountryCode = "KE", CountryName = "Kenya" },
new BlockedCountry { CountryCode = "GH", CountryName = "Ghana" },
new BlockedCountry { CountryCode = "ET", CountryName = "Ethiopia" },
new BlockedCountry { CountryCode = "UG", CountryName = "Uganda" },
new BlockedCountry { CountryCode = "SN", CountryName = "Senegal" },
new BlockedCountry { CountryCode = "ZM", CountryName = "Zambia" },
new BlockedCountry { CountryCode = "ZW", CountryName = "Zimbabwe" },
new BlockedCountry { CountryCode = "NZ", CountryName = "New Zealand" },
new BlockedCountry { CountryCode = "SE", CountryName = "Sweden" }

    };
            

            return initialCountries;
        }
    }
}
