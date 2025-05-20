using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.interfaces;

namespace BlockedCountries.Infrastructure.Repositories
{
    public class AttemptLogger : IAttemptLogger
    {
      public  List<LogAttempDTO> logger=new List<LogAttempDTO>();

        public void LogIpCheck(LogAttempDTO attempDTO)
        {
            logger.Add(attempDTO);
        }


        public PaginationResultLogs GetAll( int pageNumber=1,int pageSize=10)
        {
            var AllLogs= logger;

            var pagenation= AllLogs.Skip((pageNumber-1)  * pageSize).Take(pageSize);
            var count=pagenation.Count();


            var result = new PaginationResultLogs
            {
                Items = pagenation,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count
            };
            return result;
        }
    }
}
