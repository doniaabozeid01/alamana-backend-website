using Alamana.Service.Location;
using Alamana.Service.Location.Dtos;
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


        [HttpGet("GetCountryById/{countryId}")]
        public async Task<IActionResult> GetCountryById(int countryId)
        {
            try
            {
                var country = await _locationServices.GetCountryById(countryId);
                if (country == null)
                    return NotFound(new { message = "Country not found." });

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("UpdateCountryContact/{countryId}")]
        public async Task<IActionResult> UpdateCountryContact(int countryId, [FromBody] UpdateCountryContactDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid data." });

                var country = await _locationServices.UpdateCountryContactAsync(countryId, dto);
                if (country == null)
                    return NotFound(new { message = "Country not found or update failed." });

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetDefaultCountry")]
        public async Task<IActionResult> GetDefaultCountry()
        {
            try
            {
                var country = await _locationServices.GetDefaultCountryAsync();
                if (country == null)
                    return NotFound(new { message = "No default country configured." });

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("SetDefaultCountry/{countryId}")]
        public async Task<IActionResult> SetDefaultCountry(int countryId)
        {
            try
            {
                var country = await _locationServices.SetDefaultCountryAsync(countryId);
                if (country == null)
                    return NotFound(new { message = "Country not found or update failed." });

                return Ok(country);
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
