using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Sharig;
using BlockedCountries.Infrastructure.Repositories;
using Xunit;

namespace BlockedCountries.Infrastructure.Tests.Repositories
{
    public class AttemptLoggerTests
    {
        private readonly AttemptLogger _logger;

        public AttemptLoggerTests()
        {
            _logger = new AttemptLogger();
        }

        [Fact]
        public void LogIpCheck_ShouldAddNewEntry()
        {
            // Arrange
            var initialCount = _logger.GetAll().TotalCount;
            var attempt = new LogAttempDTO
            {
                IpAddress = "192.168.1.1",
                CountryCode = "US",
                IsBlocked = true,
                Timestamp = DateTime.UtcNow
            };

            // Act
            _logger.LogIpCheck(attempt);

            // Assert
            var result = _logger.GetAll();
            Assert.Equal(initialCount + 1, result.TotalCount);
            Assert.Contains(attempt, result.Items);
        }

        

        [Fact]
        public void GetAll_ShouldHandleEmptyLogs()
        {
            // Arrange - Clear any existing logs
            _logger.logger.Clear();

            // Act
            var result = _logger.GetAll();

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public void GetAll_ShouldReturnCorrectCount_WhenLessThanPageSize()
        {
            // Arrange - Add 3 items
            _logger.logger.Clear();
            for (int i = 0; i < 3; i++)
            {
                _logger.LogIpCheck(new LogAttempDTO
                {
                    IpAddress = $"10.0.0.{i}",
                    CountryCode = "EG",
                    IsBlocked = false,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Act - Request page with size 5
            var result = _logger.GetAll(pageSize: 5);

            // Assert
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public void GetAll_ShouldReturnLastPage_WhenPageNumberTooHigh()
        {
            // Arrange - Add 10 items
            _logger.logger.Clear();
            for (int i = 0; i < 10; i++)
            {
                _logger.LogIpCheck(new LogAttempDTO
                {
                    IpAddress = $"172.16.0.{i}",
                    CountryCode = "GB",
                    IsBlocked = true,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Act - Request page 3 with size 4
            var result = _logger.GetAll(pageNumber: 3, pageSize: 4);

            // Assert (should return last 2 items)
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.PageNumber);
        }
    }
}