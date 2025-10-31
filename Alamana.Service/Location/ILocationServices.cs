using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Location.Dtos;

namespace Alamana.Service.Location
{
    public interface ILocationServices
    {

        Task<IReadOnlyList<GetCountryDto>> GetAllCountries();





        Task<IReadOnlyList<GetGovernorateDto>> GetAllGovernorates();
        Task<IReadOnlyList<GetGovernorateDto>> GetGovernoratesByCountryId(int countryId);





        Task<IReadOnlyList<GetCityDto>> GetAllCities();
        Task<IReadOnlyList<GetCityDto>> GetCitiesByGovernorateId(int governorateId);





        Task<IReadOnlyList<GetDestrictDto>> GetAllDistricts();
        Task<IReadOnlyList<GetDestrictDto>> GetDistrictsByCityId(int cityId);






        Task<GetDestrictDto> GetDistrictById(int districtId);
        Task<GetCityDto> GetCityById(int cityId);
        Task<GetGovernorateDto> GetGovernorateById(int governorateId);
        Task<GetCountryDto> GetCountryById(int countryId);





    }


}
