using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Video.Dtos;

namespace Alamana.Service.Video
{
    public interface IVideoService
    {
        Task<List<VideoDto>> GetAllAsync();
        Task<VideoDto?> GetByIdAsync(int id);
        //Task<int> CreateAsync(CreateVideoDto dto);
        //Task UpdateAsync(int id, UpdateVideoDto dto);
        //Task DeleteAsync(int id);


        Task<int> DeleteAsync(int id);
        Task<VideoDto> UpdateAsync(int id, CreateVideoDto videoDto);
        Task<VideoDto> CreateAsync(CreateVideoDto videoDto);


        Task SetDefaultAsync(int id);
    }
}
