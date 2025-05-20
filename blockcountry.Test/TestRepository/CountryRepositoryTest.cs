using Xunit;
using BlockedCountries.Infrastructure.Repositories;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.Sharig;
using System.Collections.Generic;
using BlockedCountries.Core.interfaces;

namespace BlockedCountries.Tests
{
    public class CountryRepositoryTests
    {
        private readonly ICountryRepository _repository;

        public CountryRepositoryTests()
        {
            _repository = new CountryRepository();
        }

        [Fact]
        public void Add_ShouldAddCountry_WhenNotExists()
        {
            var country = new BlockedCountry { CountryCode = "EG", CountryName = "Test" };

            var result = _repository.Add(country);

            Assert.True(result);
            Assert.True(_repository.IsCountryBlocked("EG"));
        }

        [Fact]
        public void Add_ShouldFail_WhenCountryAlreadyExists()
        {
            var country = new BlockedCountry { CountryCode = "US", CountryName = "Test" };

            _repository.Add(country);
            var result = _repository.Add(country);

            Assert.False(result);
        }

        [Fact]
        public void IsCountryBlocked_ShouldReturnTrue_IfExists()
        {
            var country = new BlockedCountry { CountryCode = "FR", CountryName = "Test" };
            _repository.Add(country);

            var result = _repository.IsCountryBlocked("FR");

            Assert.True(result);
        }

        [Fact]
        public void IsCountryBlocked_ShouldReturnFalse_IfNotExists()
        {
            var result = _repository.IsCountryBlocked("ZZ");

            Assert.False(result);
        }

        [Fact]
        public void Remove_ShouldDeleteCountry_WhenExists()
        {
            var country = new BlockedCountry { CountryCode = "SA", CountryName = "Test" };
            _repository.Add(country);

            var result = _repository.Remove("SA");

            Assert.True(result);
            Assert.False(_repository.IsCountryBlocked("SA"));
        }

        [Fact]
        public void Remove_ShouldReturnFalse_WhenCountryDoesNotExist()
        {
            var result = _repository.Remove("XX");

            Assert.False(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAll_WithPagination()
        {
            _repository.Add(new BlockedCountry { CountryCode = "EG", CountryName = "Test" });
            _repository.Add(new BlockedCountry { CountryCode = "US", CountryName = "Test" });
            _repository.Add(new BlockedCountry { CountryCode = "FR", CountryName = "Test" });

            var pagination = new PaginationParams { PageNumber = 1, PageSize = 2 };
            var result = _repository.GetAll(pagination);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public void GetAll_ShouldFilterBySearch()
        {
            _repository.Add(new BlockedCountry { CountryCode = "EG", CountryName = "Test" });
            _repository.Add(new BlockedCountry { CountryCode = "US", CountryName = "Test" });

            var pagination = new PaginationParams { PageNumber = 1, PageSize = 10, Search = "us" };
            var result = _repository.GetAll(pagination);

            Assert.Single(result.Items);
            Assert.Equal("US", result.Items.First().CountryCode);
        }

        [Fact]
        public void seedToTest_ShouldAddInitialData()
        {
            _repository.seedToTest();

            var result = _repository.GetAll(new PaginationParams { PageNumber = 1, PageSize = 100 });

            Assert.True(result.TotalCount > 0);
        }
    }
}
