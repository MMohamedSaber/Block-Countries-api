using System.Collections.Concurrent;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Models;

namespace BlockedCountries.Infrastructure.Repositories
{
    public class TemporalBlockRepository: ITemporalBlockRepository
    {
        private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks = new();

        public bool TryAddBlock(string countryCode, int durationMinutes)
        {
            var now = DateTime.UtcNow;
            var block = new TemporalBlock
            {
                CountryCode = countryCode,
                BlockedAt = now,
                ExpiresAt = now.AddMinutes(durationMinutes)
            };

            return _temporalBlocks.TryAdd(countryCode, block);
        }

        public IEnumerable<TemporalBlock> GetExpiredBlocks()
        {
            var now = DateTime.UtcNow;
            return _temporalBlocks.Values.Where(b => b.ExpiresAt <= now);
        }

        public bool RemoveBlock(string countryCode)
        {
            return _temporalBlocks.TryRemove(countryCode, out _);
        }
        public bool IsBlocked(string countryCode)
        {
            if (_temporalBlocks.TryGetValue(countryCode, out var block))
            {
                return block.ExpiresAt > DateTime.UtcNow;
            }

            return false;
        }

    }
}
