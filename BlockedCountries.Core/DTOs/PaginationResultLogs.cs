
using BlockedCountries.Core.Entities;

namespace BlockedCountries.Core.DTOs
{
    public class PaginationResultLogs
    {
        public IEnumerable<LogAttempDTO> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
