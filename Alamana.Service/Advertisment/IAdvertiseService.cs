using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Advertisment.Dtos;

namespace Alamana.Service.Advertisment
{
    public interface IAdvertiseService
    {
        Task<int> DeleteAdvertisement(int id);
        Task<AdvertiseDto> GetAdvertisementById(int id);
        Task<IReadOnlyList<AdvertiseDto>> GetAllAdvertisements();
        Task<AdvertiseDto> AddAdvertise(AddAdvertise imageDto);
        Task<AdvertiseDto> UpdateAdvertise(int id, AddAdvertise imageDto);
    }
}
