using BlockedCountries.API.Controllers;
using BlockedCountries.API.helper;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Sharig;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BlockedCountries.API.Tests.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Mock<ICountryRepository> _mockCountryRepo;
        private readonly Mock<ITemporalBlockRepository> _mockTempBlockRepo;
        private readonly CountriesController _controller;

        public CountriesControllerTests()
        {
            _mockCountryRepo = new Mock<ICountryRepository>();
            _mockTempBlockRepo = new Mock<ITemporalBlockRepository>();
            _controller = new CountriesController(_mockCountryRepo.Object, _mockTempBlockRepo.Object);
        }

        [Fact]
        public void Add_WhenCountryNotBlocked_ReturnsOk()
        {
            // Arrange
            var country = new BlockedCountry { CountryCode = "US" };
            _mockCountryRepo.Setup(x => x.Add(country)).Returns(true);

            // Act
            var result = _controller.add(country);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Country blocked Successfully", okResult.Value);
        }

        [Fact]
        public void Add_WhenCountryAlreadyBlocked_ReturnsBadRequest()
        {
            // Arrange
            var country = new BlockedCountry { CountryCode = "US" };
            _mockCountryRepo.Setup(x => x.Add(country)).Returns(false);

            // Act
            var result = _controller.add(country);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Country is Already Blocked", badRequestResult.Value);
        }

        [Fact]
        public void Get_WhenCountriesExist_ReturnsPaginatedResult()
        {
            // Arrange
            var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var expectedResult = new PaginatedResult
            {
                Items = new List<BlockedCountry> { new BlockedCountry { CountryCode = "US" } },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _mockCountryRepo.Setup(x => x.GetAll(pagination)).Returns(expectedResult);

            // Act
            var result = _controller.get(pagination);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<PaginatedResult>(okResult.Value);
            Assert.Equal(expectedResult.Items, returnedResult.Items);
            Assert.Equal(expectedResult.TotalCount, returnedResult.TotalCount);
        }

        [Fact]
        public void Get_WhenNoCountries_ReturnsBadRequest()
        {
            // Arrange
            var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };
            _mockCountryRepo.Setup(x => x.GetAll(pagination)).Returns((PaginatedResult)null);

            // Act
            var result = _controller.get(pagination);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("no countries blocked", ((ResponseApi)badRequestResult.Value).Message);
        }

        [Fact]
        public void Delete_WhenCountryExists_ReturnsSuccess()
        {
            // Arrange
            var countryCode = "US";
            _mockCountryRepo.Setup(x => x.Remove(countryCode)).Returns(true);

            // Act
            var result = _controller.delete(countryCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ((ResponseApi)okResult.Value).StatusCode);
        }

        [Fact]
        public void Delete_WhenCountryNotExists_ReturnsBadRequest()
        {
            // Arrange
            var countryCode = "XX";
            _mockCountryRepo.Setup(x => x.Remove(countryCode)).Returns(false);

            // Act
            var result = _controller.delete(countryCode);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, ((ResponseApi)badRequestResult.Value).StatusCode);
        }
        [Fact]
        public void TemporarilyBlockCountry_WhenNotBlocked_ReturnsOkWithExpiry()
        {
            // Arrange
            var request = new TemporalBlockDTO { CountryCode = "US", DurationMinutes = 30 };
            _mockTempBlockRepo.Setup(x => x.TryAddBlock(request.CountryCode, request.DurationMinutes))
                             .Returns(true);

            // Act
            var result = _controller.TemporarilyBlockCountry(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Option 1: If returning anonymous type
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal(request.CountryCode, (string)response.GetType().GetProperty("CountryCode").GetValue(response));
            Assert.True((DateTime)response.GetType().GetProperty("ExpiresAt").GetValue(response) > DateTime.UtcNow);

          
        }

        [Fact]
        public void TemporarilyBlockCountry_WhenAlreadyBlocked_ReturnsConflict()
        {
            // Arrange
            var request = new TemporalBlockDTO { CountryCode = "US", DurationMinutes = 30 };
            _mockTempBlockRepo.Setup(x => x.TryAddBlock(request.CountryCode, request.DurationMinutes)).Returns(false);

            // Act
            var result = _controller.TemporarilyBlockCountry(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, ((ResponseApi)conflictResult.Value).StatusCode);
        }
    }
}