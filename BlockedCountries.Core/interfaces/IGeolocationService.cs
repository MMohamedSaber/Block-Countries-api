
using BlockedCountries.Core.DTOs;

namespace BlockedCountries.Core.interfaces
{
    public interface IGeolocationService
    {
        Task<GeoLocationResponse> GetGeoDataAsync(string ipAddress);
    }
}
