using BlockedCountries.API.helper;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.Entities;
using BlockedCountries.Core.interfaces;
using BlockedCountries.Core.Sharig;
using BlockedCountries.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace BlockedCountries.API.Controllers
{
    
    public class CountriesController  : BaseController
    {
        private readonly ICountryRepository _country;
        private readonly ITemporalBlockRepository _temporalBlockRepository;
        public CountriesController(ICountryRepository country, ITemporalBlockRepository emporalBlockRepository)
        {
            _country = country;
            _temporalBlockRepository = emporalBlockRepository;
        }

        [HttpPost("blocked-add")]
        public  IActionResult add(BlockedCountry blockedCountry)
        { 
            var IsBlocked= _country.Add(blockedCountry);
            if (!IsBlocked)
            {
                return BadRequest("Country is Already Blocked");
            }
            return Ok("Country blocked Successfully");
        }

        [HttpGet("blocked-getall")]
        public  IActionResult get([FromQuery] PaginationParams paginationParams)
        {
            var blockedCountries= _country.GetAll(paginationParams);
            if (blockedCountries==null)
                return BadRequest(new ResponseApi(400, "no countries blocked"));

            return Ok(blockedCountries);
        }

        [HttpDelete]
        public  IActionResult delete(string countrycode)
        {

            var IsDeleted =  _country.Remove(countrycode);
            if (IsDeleted)
            {
                return Ok(new ResponseApi(200, "Country Deleted Successfuly"));
            }

            return BadRequest(new ResponseApi(400));
        }


        [HttpPost("temporal-block")]
        public IActionResult TemporarilyBlockCountry( [FromBody] TemporalBlockDTO request)
        {
            
            if (!_temporalBlockRepository.TryAddBlock(request.CountryCode, request.DurationMinutes))
            {
                return Conflict(new ResponseApi(409, "Country is already temporarily blocked"));
            }

            return Ok(new
            {
                request.CountryCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
            });
        }

    }
}
