using System.Net;
using BlockedCountries.API.helper;
using BlockedCountries.Core.DTOs;
using BlockedCountries.Core.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockedCountries.API.Controllers
{
    public class ipController : BaseController
    {
        protected readonly IGeolocationService _geoService;
        protected readonly ICountryRepository _countryRepository;
        protected readonly IAttemptLogger _attemptLogger;

        public ipController(IGeolocationService geoService, ICountryRepository countryRepository, IAttemptLogger attemptLogger)
        {
            _geoService = geoService;
            _countryRepository = countryRepository;
            _attemptLogger = attemptLogger;
        }

        [HttpGet("lookUp")]
        public async Task<IActionResult> LookupIp([FromQuery] string ipAddress = null)
        {
            try
            {
                //  Get caller's IP automatically
                ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();
                ipAddress = "8.8.8.8";

                if (string.IsNullOrEmpty(ipAddress))
                    return BadRequest( new ResponseApi(400,"Could not determine IP address"));

                if (!IPAddress.TryParse(ipAddress, out _))
                    return BadRequest(new ResponseApi(400, "Invalid IP address format"));

                var geoData = await _geoService.GetGeoDataAsync(ipAddress);


                var isBlocked =  _countryRepository.IsCountryBlocked(geoData.CountryCode);

                if (isBlocked)
                    geoData.IsBlocked = true;

                return Ok(geoData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Geolocation service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckIfIpIsBlocked()
        {
            try
            {
                //  Get caller's IP automatically
                 var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                  ipAddress = "8.8.8.8";


                if (string.IsNullOrEmpty(ipAddress))
                    return BadRequest(new ResponseApi(400, "Could not determine your IP address"));
                    
                //  Validate IP format
                if (!IPAddress.TryParse(ipAddress, out _))
                    return BadRequest(new ResponseApi(400, "Your IP address appears to be invalid"));
                

                //  Get country code from third-party API
                var geoData = await _geoService.GetGeoDataAsync(ipAddress);

                var isBlocked =  _countryRepository.IsCountryBlocked(geoData.CountryCode);

                if (isBlocked)
                    geoData.IsBlocked = true;


                var agent=HttpContext.Request.Headers["User-Agent"].ToString();
                var logAttempt = new LogAttempDTO
                {
                     IpAddress=ipAddress,
                      CountryCode=geoData.CountryCode,
                       CountryName=geoData.country_name,
                        IsBlocked=isBlocked   ,
                         userAgent= agent,
                          Timestamp=DateTime.UtcNow
                };

                //  Log the attempt (implementation shown later)
                 _attemptLogger.LogIpCheck(logAttempt);

                return Ok(logAttempt);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseApi(400,ex.Message));
            }
            
        }

        [HttpPost("seed-blockedCountry-ToTest")]
        public IActionResult seed()
        {
            _countryRepository.seedToTest();
            return Ok(new ResponseApi(200));
        }
    }
}
