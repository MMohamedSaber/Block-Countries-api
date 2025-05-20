
using System.ComponentModel;
using BlockedCountries.Core.interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlockedCountries.Infrastructure.Repositories.Services
{
    public class TemporalBlockCleanupService : BackgroundService
    {
       
            private readonly ITemporalBlockRepository _repository;
            private readonly ILogger<TemporalBlockCleanupService> _logger;
            private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

            public TemporalBlockCleanupService(
                ITemporalBlockRepository repository,
                ILogger<TemporalBlockCleanupService> logger)
            {
                _repository = repository;
                _logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var expired = _repository.GetExpiredBlocks().ToList();
                        foreach (var block in expired)
                        {
                            _repository.RemoveBlock(block.CountryCode);
                            _logger.LogInformation(
                                "Automatically unblocked {CountryCode} after temporary block expired",
                                block.CountryCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error cleaning up temporal blocks");
                    }

                    await Task.Delay(_interval, stoppingToken);
                }
            }
        }
    }

