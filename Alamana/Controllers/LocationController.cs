using Alamana.Service.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationServices _locationServices;

        public LocationController(ILocationServices locationServices)
        {
            _locationServices = locationServices;
        }

        [HttpGet("GetAllCountries")]
        public async Task<IActionResult> GetAllCountries ()
        {
            try
            {
                var countries = await _locationServices.GetAllCountries();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetAllGovernorates")]
        public async Task<IActionResult> GetAllGovernorates()
        {
            try
            {
                var Governorates = await _locationServices.GetAllGovernorates();
                return Ok(Governorates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetGovernoratesByCountryId/{CountryId}")]
        public async Task<IActionResult> GetGovernoratesByCountryId(int CountryId)
        {
            try
            {
                var Governorates = await _locationServices.GetGovernoratesByCountryId(CountryId);
                return Ok(Governorates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpGet("GetAllCities")]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var cities = await _locationServices.GetAllCities();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpGet("GetCitiesByGovernorateId/{GovernorateId}")]
        public async Task<IActionResult> GetCitiesByGovernorateId(int GovernorateId)
        {
            try
            {
                var cities = await _locationServices.GetCitiesByGovernorateId(GovernorateId);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetAllDistricts")]
        public async Task<IActionResult> GetAllDistricts()
        {
            try
            {
                var districts = await _locationServices.GetAllDistricts();
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }




        [HttpGet("GetDistrictsByCityId/{cityId}")]
        public async Task<IActionResult> GetDistrictsByCityId(int cityId)
        {
            try
            {
                var districts = await _locationServices.GetDistrictsByCityId(cityId);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
