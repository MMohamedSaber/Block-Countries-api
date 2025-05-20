
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.Sharig;

namespace BlockedCountries.Core.interfaces
{
    public interface ICountryRepository
    {
        bool Add(BlockedCountry entity);
        PaginatedResult GetAll(PaginationParams pagination);
        bool Remove(string countryCode);
        bool IsCountryBlocked(string countryCode);
        void seedToTest();
    }
}
