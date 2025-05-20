
using System.ComponentModel.DataAnnotations;

namespace BlockedCountries.Core.Sharig
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;

        private int MaxPageSize { get; set; } = 6;

        private int _pageSize = 3;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public string? Search { get; set; }
    }
}
