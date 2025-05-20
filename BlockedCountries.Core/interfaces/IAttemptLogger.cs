
using BlockedCountries.Core.DTOs;

namespace BlockedCountries.Core.interfaces
{
    public interface IAttemptLogger
    {
        void LogIpCheck(LogAttempDTO attempDTO);
        PaginationResultLogs GetAll(int pageNumber = 1, int pageSize = 10);
    }
}
