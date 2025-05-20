
using BlockedCountries.API.Controllers;
using BlockedCountries.API.helper;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace BlockedCountries.API.Tests.Controllers
{
    public class IpControllerTests
    {
        private readonly Mock<IGeolocationService> _mockGeoService;
        private readonly Mock<ICountryRepository> _mockCountryRepo;
        private readonly Mock<IAttemptLogger> _mockAttemptLogger;
        private readonly ipController _controller;

        public IpControllerTests()
        {
            _mockGeoService = new Mock<IGeolocationService>();
            _mockCountryRepo = new Mock<ICountryRepository>();
            _mockAttemptLogger = new Mock<IAttemptLogger>();

            _controller = new ipController(
                _mockGeoService.Object,
                _mockCountryRepo.Object,
                _mockAttemptLogger.Object);

            // Setup controller context for HTTP requests
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task LookupIp_WithValidIp_ReturnsGeoData()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            var expectedGeoData = new GeoLocationResponse
            {
                CountryCode = "US",
                country_name = "United States",
                Ip = ipAddress
            };

            _mockGeoService.Setup(x => x.GetGeoDataAsync(ipAddress))
                          .ReturnsAsync(expectedGeoData);
            _mockCountryRepo.Setup(x => x.IsCountryBlocked("US")).Returns(false);

            // Act
            var result = await _controller.LookupIp(ipAddress);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GeoLocationResponse>(okResult.Value);
            Assert.Equal("US", response.CountryCode);
            Assert.False(response.IsBlocked);
        }

        [Fact]
        public async Task LookupIp_WithBlockedCountry_SetsIsBlocked()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            var geoData = new GeoLocationResponse { CountryCode = "IR" };

            _mockGeoService.Setup(x => x.GetGeoDataAsync(ipAddress))
                          .ReturnsAsync(geoData);
            _mockCountryRepo.Setup(x => x.IsCountryBlocked("IR")).Returns(true);

            // Act
            var result = await _controller.LookupIp(ipAddress);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GeoLocationResponse>(okResult.Value);
            Assert.True(response.IsBlocked);
        }

       

        [Fact]
        public async Task CheckIfIpIsBlocked_WithValidIp_LogsAttempt()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            var geoData = new GeoLocationResponse
            {
                CountryCode = "US",
                country_name = "United States"
            };

            _mockGeoService.Setup(x => x.GetGeoDataAsync(ipAddress))
                          .ReturnsAsync(geoData);
            _mockCountryRepo.Setup(x => x.IsCountryBlocked("US")).Returns(false);

            // Setup HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
            httpContext.Request.Headers["User-Agent"] = "TestAgent";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.CheckIfIpIsBlocked();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockAttemptLogger.Verify(x => x.LogIpCheck(It.Is<LogAttempDTO>(dto =>
                dto.IpAddress == ipAddress &&
                dto.CountryCode == "US" &&
                dto.userAgent == "TestAgent")), Times.Once);
        }

        [Fact]
        public async Task CheckIfIpIsBlocked_WithBlockedCountry_ReturnsBlockedStatus()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            var geoData = new GeoLocationResponse
            {
                CountryCode = "IR",
                country_name = "Iran"
            };

            _mockGeoService.Setup(x => x.GetGeoDataAsync(ipAddress))
                          .ReturnsAsync(geoData);
            _mockCountryRepo.Setup(x => x.IsCountryBlocked("IR")).Returns(true);

            // Setup HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.CheckIfIpIsBlocked();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LogAttempDTO>(okResult.Value);
            Assert.True(response.IsBlocked);
        }

        [Fact]
        public void SeedBlockedCountry_ReturnsOk()
        {
            // Arrange
            _mockCountryRepo.Setup(x => x.seedToTest());

            // Act
            var result = _controller.seed();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ((ResponseApi)okResult.Value).StatusCode);
        }

        [Fact]
        public async Task LookupIp_WhenGeoServiceFails_ReturnsServiceError()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            _mockGeoService.Setup(x => x.GetGeoDataAsync(ipAddress))
                          .ThrowsAsync(new HttpRequestException("Service unavailable"));

            // Act
            var result = await _controller.LookupIp(ipAddress);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(502, statusCodeResult.StatusCode);
        }
    }
}