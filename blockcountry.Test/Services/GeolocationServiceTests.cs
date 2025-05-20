using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Infrastructure.Repositories.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace BlockedCountries.Infrastructure.Tests.Services
{
    public class GeolocationServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<GeolocationService>> _mockLogger;
        private readonly GeolocationService _service;

        public GeolocationServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<GeolocationService>>();

            _mockConfig.Setup(x => x["GeoLocationApi:BaseUrl"])
                      .Returns("https://api.example.com/");
            _mockConfig.Setup(x => x["GeoLocationApi:ApiKey"])
                      .Returns("test-api-key");
            _mockConfig.Setup(x => x["GeoLocationApi:TimeoutSeconds"])
                      .Returns("30");

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.example.com/")
            };

            _service = new GeolocationService(httpClient, _mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetGeoDataAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            var expectedResponse = new GeoLocationResponse
            {
                CountryCode = "US",
                country_name = "United States",
                Ip = ipAddress
            };

            SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

            // Act
            var result = await _service.GetGeoDataAsync(ipAddress);

            // Assert
            Assert.Equal(expectedResponse.CountryCode, result.CountryCode);
            Assert.Equal(expectedResponse.country_name, result.country_name);
            Assert.Equal(ipAddress, result.Ip);
        }

        [Fact]
        public async Task GetGeoDataAsync_HandlesRateLimit()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            SetupHttpResponse(HttpStatusCode.TooManyRequests);

            // Act
            var result = await _service.GetGeoDataAsync(ipAddress);

            // Assert
            Assert.Equal("XX", result.CountryCode);
            VerifyLog(LogLevel.Warning, "Rate limit exceeded");
        }

        [Fact]
        public async Task GetGeoDataAsync_HandlesServerError()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            SetupHttpResponse(HttpStatusCode.InternalServerError);

            // Act
            var result = await _service.GetGeoDataAsync(ipAddress);

            // Assert
            Assert.Equal("XX", result.CountryCode);
            VerifyLog(LogLevel.Error, "GeoLocation API returned non-success status");
        }

        [Fact]
        public async Task GetGeoDataAsync_HandlesExceptions()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetGeoDataAsync(ipAddress));
            VerifyLog(LogLevel.Error, "Error fetching geo data");
        }

        [Fact]
        public async Task GetGeoDataAsync_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var ipAddress = "8.8.8.8";
            SetupInvalidJsonResponse();

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _service.GetGeoDataAsync(ipAddress));
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, GeoLocationResponse response = null)
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (response != null)
            {
                httpResponse.Content = new StringContent(
                    JsonSerializer.Serialize(response),
                    Encoding.UTF8,
                    "application/json");
            }

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
        }

        private void SetupInvalidJsonResponse()
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
                });
        }

        private void VerifyLog(LogLevel logLevel, string expectedMessagePart)
        {
            _mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessagePart)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}