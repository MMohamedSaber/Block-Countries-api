using System;
using BlockedCountries.Core.Models;
using BlockedCountries.Infrastructure.Repositories;
using Xunit;

namespace BlockedCountries.Infrastructure.Tests.Repositories
{
    public class TemporalBlockRepositoryTests : IDisposable
    {
        private readonly TemporalBlockRepository _repository;

        public TemporalBlockRepositoryTests()
        {
            _repository = new TemporalBlockRepository();
        }

        public void Dispose()
        {
            // Clean up between tests if needed
        }

        [Fact]
        public void TryAddBlock_ShouldAddNewBlock_WhenCountryNotBlocked()
        {
            // Arrange
            var countryCode = "US";
            var durationMinutes = 30;

            // Act
            var result = _repository.TryAddBlock(countryCode, durationMinutes);

            // Assert
            Assert.True(result);
            Assert.True(_repository.IsBlocked(countryCode));
        }

        [Fact]
        public void TryAddBlock_ShouldReturnFalse_WhenCountryAlreadyBlocked()
        {
            // Arrange
            var countryCode = "US";
            var durationMinutes = 30;
            _repository.TryAddBlock(countryCode, durationMinutes);

            // Act
            var result = _repository.TryAddBlock(countryCode, durationMinutes);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetExpiredBlocks_ShouldReturnOnlyExpiredBlocks()
        {
            // Arrange
            var expiredBlock = new TemporalBlock
            {
                CountryCode = "US",
                BlockedAt = DateTime.UtcNow.AddMinutes(-60),
                ExpiresAt = DateTime.UtcNow.AddMinutes(-30)
            };
            var activeBlock = new TemporalBlock
            {
                CountryCode = "CA",
                BlockedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            _repository.TryAddBlock(expiredBlock.CountryCode, -30); // Already expired
            _repository.TryAddBlock(activeBlock.CountryCode, 30);

            // Act
            var expiredBlocks = _repository.GetExpiredBlocks().ToList();

            // Assert
            Assert.Single(expiredBlocks);
            Assert.Equal("US", expiredBlocks[0].CountryCode);
        }

        [Fact]
        public void RemoveBlock_ShouldRemoveExistingBlock()
        {
            // Arrange
            var countryCode = "US";
            _repository.TryAddBlock(countryCode, 30);

            // Act
            var result = _repository.RemoveBlock(countryCode);

            // Assert
            Assert.True(result);
            Assert.False(_repository.IsBlocked(countryCode));
        }

        [Fact]
        public void RemoveBlock_ShouldReturnFalse_WhenBlockDoesNotExist()
        {
            // Arrange
            var countryCode = "XX";

            // Act
            var result = _repository.RemoveBlock(countryCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsBlocked_ShouldReturnTrue_ForActiveBlock()
        {
            // Arrange
            var countryCode = "US";
            _repository.TryAddBlock(countryCode, 30);

            // Act
            var result = _repository.IsBlocked(countryCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsBlocked_ShouldReturnFalse_ForExpiredBlock()
        {
            // Arrange
            var countryCode = "US";
            _repository.TryAddBlock(countryCode, -30); // Already expired

            // Act
            var result = _repository.IsBlocked(countryCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsBlocked_ShouldReturnFalse_ForNonExistentBlock()
        {
            // Arrange
            var countryCode = "XX";

            // Act
            var result = _repository.IsBlocked(countryCode);

            // Assert
            Assert.False(result);
        }
    }
}