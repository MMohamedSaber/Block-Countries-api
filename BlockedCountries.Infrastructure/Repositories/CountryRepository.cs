using System.Collections.Concurrent;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Sharig;

namespace BlockedCountries.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ConcurrentDictionary<string, BlockedCountry> _countries = new();

        public CountryRepository()
        {
           
        }
        public void seedToTest()
        {
            var initialData = SeedBlockedCountry.init();
            foreach (var item in initialData)
            {
                _countries.TryAdd(item.CountryCode, item);
            }
        }
        public bool Add(BlockedCountry country)
        {
            if (_countries.ContainsKey(country.CountryCode))
                return false;

            return _countries.TryAdd(country.CountryCode, country);
        }
   
        public bool IsCountryBlocked(string CountryCode)
        {
            return _countries.ContainsKey(CountryCode);
        }

        public  PaginatedResult GetAll(PaginationParams pagination)
        {
            // Apply filtering
            var query = _countries.Values.AsEnumerable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(c => c.CountryCode.Contains(
                    pagination.Search, StringComparison.OrdinalIgnoreCase));
            }
            // Get total count after filtering
            var totalCount = query.Count();
            // Apply pagination
            var items = query
                .OrderBy(c => c.CountryCode)
                .Skip(( pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            return  new PaginatedResult
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }


        public bool Remove(string countryCode)
        {
            bool removed = _countries.TryRemove(countryCode.ToUpper(), out _);
            return removed;
        }
    }
}
