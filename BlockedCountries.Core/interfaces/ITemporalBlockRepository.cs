
using BlockedCountries.Core.Models;

namespace BlockedCountries.Core.interfaces
{
    public interface ITemporalBlockRepository
    {
        bool TryAddBlock(string countryCode, int durationMinutes);
        IEnumerable<TemporalBlock> GetExpiredBlocks();
        bool RemoveBlock(string countryCode);
        bool IsBlocked(string countryCode);
    }
}
