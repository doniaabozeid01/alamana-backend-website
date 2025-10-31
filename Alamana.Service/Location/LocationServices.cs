using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Location.Dtos;
using Alamana.Service.Product.Dtos;
using AutoMapper;
using Org.BouncyCastle.Security;

namespace Alamana.Service.Location
{
    public class LocationServices :ILocationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IReadOnlyList<GetCountryDto>> GetAllCountries()
        {
            var countries = await _unitOfWork.Repository<country>().GetAllAsync();
            var mappedCountries = _mapper.Map<IReadOnlyList<GetCountryDto>>(countries);
            return mappedCountries;
        }


        public async Task<IReadOnlyList<GetGovernorateDto>> GetAllGovernorates()
        {
            var Governorates = await _unitOfWork.Repository<Governorate>().GetAllAsync();
            var mappedGovernorates = _mapper.Map<IReadOnlyList<GetGovernorateDto>>(Governorates);
            return mappedGovernorates;
        }



        public async Task<IReadOnlyList<GetGovernorateDto>> GetGovernoratesByCountryId(int countryId)
        {
            var Governorates = await _unitOfWork.Repository<Governorate>().GetGovernoratesByCountryId(countryId);
            var mappedGovernorates = _mapper.Map<IReadOnlyList<GetGovernorateDto>>(Governorates);
            return mappedGovernorates;
        }



        public async Task<IReadOnlyList<GetCityDto>> GetAllCities()
        {
            var Cities = await _unitOfWork.Repository<city>().GetAllAsync();
            var mappedCities = _mapper.Map<IReadOnlyList<GetCityDto>>(Cities);
            return mappedCities;
        }







        public async Task<IReadOnlyList<GetCityDto>> GetCitiesByGovernorateId(int governorateId)
        {
            var Cities = await _unitOfWork.Repository<city>().GetCitiesByGovernorateId(governorateId);
            var mappedCities = _mapper.Map<IReadOnlyList<GetCityDto>>(Cities);
            return mappedCities;
        }







        public async Task<IReadOnlyList<GetDestrictDto>> GetAllDistricts()
        {
            var Districts = await _unitOfWork.Repository<District>().GetAllAsync();
            var mappedDistricts = _mapper.Map<IReadOnlyList<GetDestrictDto>>(Districts);
            return mappedDistricts;
        }





        public async Task<IReadOnlyList<GetDestrictDto>> GetDistrictsByCityId(int cityId)
        {
            var Districts = await _unitOfWork.Repository<District>().GetDistrictsByCityId(cityId);
            var mappedDistricts = _mapper.Map<IReadOnlyList<GetDestrictDto>>(Districts);
            return mappedDistricts;
        }






        public async Task<GetCountryDto> GetCountryById(int countryId)
        {
            var country = await _unitOfWork.Repository<country>().GetByIdAsync(countryId);
            var mappedCountry = _mapper.Map<GetCountryDto>(country);
            return mappedCountry;
        }



        public async Task<GetGovernorateDto> GetGovernorateById (int governorateId)
        {
            var Governorate = await _unitOfWork.Repository<Governorate>().GetByIdAsync(governorateId);
            var mappedGovernorate = _mapper.Map<GetGovernorateDto>(Governorate);
            return mappedGovernorate;
        }



        public async Task<GetCityDto> GetCityById(int cityId)
        {
            var city = await _unitOfWork.Repository<city>().GetByIdAsync(cityId);
            var mappedCity = _mapper.Map<GetCityDto>(city);
            return mappedCity;
        }



        public async Task<GetDestrictDto> GetDistrictById (int districtId)
        {
            var district = await _unitOfWork.Repository<District>().GetByIdAsync(districtId);
            var mappedDistrict = _mapper.Map<GetDestrictDto>(district);
            return mappedDistrict;
        }


    }

}
