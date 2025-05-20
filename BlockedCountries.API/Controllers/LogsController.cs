using BlockedCountries.Core.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountries.API.Controllers
{
    public class LogsController : BaseController
    {
        private readonly IAttemptLogger _logger;

        public LogsController(IAttemptLogger logger)
        {
            _logger = logger;
        }

        [HttpGet("getAll")]
        public IActionResult get(int pageNumber,int pageSize)
        {
            var result =  _logger.GetAll(pageNumber,pageSize);
            
            if (result is not null) 
            return Ok(result);

            return BadRequest(404);
        }
    }
}
