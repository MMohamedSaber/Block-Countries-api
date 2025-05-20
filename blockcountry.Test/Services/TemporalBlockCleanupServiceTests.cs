using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Models;
using BlockedCountries.Infrastructure.Repositories.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockedCountries.Infrastructure.Tests.Services
{
    public class TemporalBlockCleanupServiceTests
    {
        private readonly Mock<ITemporalBlockRepository> _mockRepo;
        private readonly Mock<ILogger<TemporalBlockCleanupService>> _mockLogger;
        private readonly TemporalBlockCleanupService _service;

        public TemporalBlockCleanupServiceTests()
        {
            _mockRepo = new Mock<ITemporalBlockRepository>();
            _mockLogger = new Mock<ILogger<TemporalBlockCleanupService>>();
            _service = new TemporalBlockCleanupService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_RemovesExpiredTemporalBlocks()
        {
            // Arrange
            var expiredBlocks = new List<TemporalBlock>
            {
                new TemporalBlock { CountryCode = "US", BlockedAt = DateTime.UtcNow.AddHours(-2), ExpiresAt = DateTime.UtcNow.AddHours(-1) },
                new TemporalBlock { CountryCode = "GB", BlockedAt = DateTime.UtcNow.AddMinutes(-90), ExpiresAt = DateTime.UtcNow.AddMinutes(-30) }
            };

            _mockRepo.Setup(x => x.GetExpiredBlocks())
                     .Returns(expiredBlocks);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100); // Cancel after short delay

            // Act
            await _service.StartAsync(cts.Token);
            await Task.Delay(200); // Give it time to process

            // Assert
            _mockRepo.Verify(x => x.RemoveBlock("US"), Times.Once);
            _mockRepo.Verify(x => x.RemoveBlock("GB"), Times.Once);

            VerifyLogInformation($"Automatically unblocked US after temporary block expired");
            VerifyLogInformation($"Automatically unblocked GB after temporary block expired");
        }

        [Fact]
        public async Task ExecuteAsync_HandlesMixedActiveAndExpiredBlocks()
        {
            // Arrange
            var mixedBlocks = new List<TemporalBlock>
            {
                new TemporalBlock { CountryCode = "US", ExpiresAt = DateTime.UtcNow.AddHours(-1) }, // Expired
                new TemporalBlock { CountryCode = "JP", ExpiresAt = DateTime.UtcNow.AddHours(1) }   // Active
            };

            _mockRepo.Setup(x => x.GetExpiredBlocks())
                     .Returns(mixedBlocks.Where(b => b.ExpiresAt <= DateTime.UtcNow));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await _service.StartAsync(cts.Token);
            await Task.Delay(200);

            // Assert
            _mockRepo.Verify(x => x.RemoveBlock("US"), Times.Once);
            _mockRepo.Verify(x => x.RemoveBlock("JP"), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_HandlesRepositoryExceptions()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetExpiredBlocks())
                     .Throws(new Exception("Database connection failed"));

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await _service.StartAsync(cts.Token);
            await Task.Delay(200);

            // Assert
            VerifyLogError("Error cleaning up temporal blocks");
        }

        [Fact]
        public async Task ExecuteAsync_CompletesImmediately_WhenCancelled()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            // Act
            await _service.StartAsync(cts.Token);

            // Assert
            _mockRepo.Verify(x => x.GetExpiredBlocks(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_RunsRepeatedlyAtInterval()
        {
            // Arrange
            var callCount = 0;
            _mockRepo.Setup(x => x.GetExpiredBlocks())
                     .Callback(() => callCount++)
                     .Returns(new List<TemporalBlock>());

            var cts = new CancellationTokenSource();
            cts.CancelAfter(350); // Cancel after 350ms (should allow 2 iterations)

            // Act
            await _service.StartAsync(cts.Token);
            await Task.Delay(400);

            // Assert
            Assert.InRange(callCount, 1, 2); // Should run 1-2 times in 350ms
        }

        private void VerifyLogInformation(string expectedMessage)
        {
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private void VerifyLogError(string expectedMessage)
        {
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
}