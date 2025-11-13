using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Repository.Repositories;
using Alamana.Service.Video.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Service.Video
{
    public class VideosService :IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AlamanaBbContext _ctx; // لعمليات جماعية بسيطة

        public VideosService(IUnitOfWork unitOfWork, IMapper mapper, AlamanaBbContext ctx)
        {
            _unitOfWork = unitOfWork; _mapper = mapper; _ctx = ctx;
        }

        public async Task<List<VideoDto>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Videos>().GetAllAsync();
            return _mapper.Map<List<VideoDto>>(list);
        }

        public async Task<VideoDto?> GetByIdAsync(int id)
        {
            var v = await _unitOfWork.Repository<Videos>().GetByIdAsync(id);
            return v is null ? null : _mapper.Map<VideoDto>(v);
        }

        public async Task<int> CreateAsync(CreateVideoDto dto)
        {
            var entity = _mapper.Map<Videos>(dto);

            if (dto.IsDefault)
            {
                var current = await _ctx.Videos.Where(x => x.IsDefault).ToListAsync();
                foreach (var c in current) c.IsDefault = false;
            }

            await _unitOfWork.Repository<Videos>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(int id, UpdateVideoDto dto)
        {
            var entity = await _unitOfWork.Repository<Videos>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Video not found");
            _mapper.Map(dto, entity);

            if (dto.IsDefault)
            {
                var others = await _ctx.Videos.Where(x => x.IsDefault && x.Id != id).ToListAsync();
                foreach (var c in others) c.IsDefault = false;
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Videos>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Video not found");
            _unitOfWork.Repository<Videos>().Delete(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task SetDefaultAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Videos>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Video not found");
            var current = await _ctx.Videos.Where(x => x.IsDefault && x.Id != id).ToListAsync();
            foreach (var c in current) c.IsDefault = false;
            entity.IsDefault = true;
            await _unitOfWork.CompleteAsync();
        }

    }
}
