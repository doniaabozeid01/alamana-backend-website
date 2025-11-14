using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Data.Migrations;
using Alamana.Repository.Interfaces;
using Alamana.Repository.Repositories;
using Alamana.Service.Category.Dtos;
using Alamana.Service.Product.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using Alamana.Service.Video.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Alamana.Service.Video
{
    public class VideosService :IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISaveAndDeleteImageService _imageService;

        private readonly AlamanaBbContext _ctx; // لعمليات جماعية بسيطة

        public VideosService(IUnitOfWork unitOfWork, IMapper mapper, AlamanaBbContext ctx, ISaveAndDeleteImageService imageService)
        {
            _unitOfWork = unitOfWork; _mapper = mapper; _ctx = ctx; _imageService = imageService;
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

        //public async Task<int> CreateAsync(CreateVideoDto dto)
        //{
        //    var entity = _mapper.Map<Videos>(dto);

        //    if (dto.IsDefault)
        //    {
        //        var current = await _ctx.Videos.Where(x => x.IsDefault).ToListAsync();
        //        foreach (var c in current) c.IsDefault = false;
        //    }

        //    await _unitOfWork.Repository<Videos>().AddAsync(entity);
        //    await _unitOfWork.CompleteAsync();
        //    return entity.Id;
        //}






        //public async Task<int> CreateAsync(CreateVideoDto dto)
        //{
        //    var entity = _mapper.Map<Videos>(dto);

        //    if (dto.IsDefault)
        //    {
        //        var current = await _ctx.Videos.Where(x => x.IsDefault).ToListAsync();
        //        foreach (var c in current) c.IsDefault = false;
        //    }

        //    await _unitOfWork.Repository<Videos>().AddAsync(entity);
        //    await _unitOfWork.CompleteAsync();



        //    return entity.Id;
        //}
        public async Task<VideoDto> CreateAsync(CreateVideoDto videoDto)
        {
            var imageUrl = videoDto.video != null ? await _imageService.UploadToCloudinary(videoDto.video) : null;

            var video = new Videos
            {
                IsDefault = videoDto.IsDefault,
                Url = imageUrl
            };

            if(videoDto.IsDefault == true)
            {
                var current = await _ctx.Videos.Where(x => x.IsDefault).ToListAsync();
                foreach (var c in current) c.IsDefault = false;
            }


            await _unitOfWork.Repository<Videos>().AddAsync(video);
            var status = await _unitOfWork.CompleteAsync();

            return  _mapper.Map<VideoDto>(video);
        }


        //public async Task UpdateAsync (int id, UpdateVideoDto dto)
        //{
        //    var entity = await _unitOfWork.Repository<Videos>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Video not found");
        //    _mapper.Map(dto, entity);

        //    if (dto.IsDefault)
        //    {
        //        var others = await _ctx.Videos.Where(x => x.IsDefault && x.Id != id).ToListAsync();
        //        foreach (var c in others) c.IsDefault = false;
        //    }

        //    await _unitOfWork.CompleteAsync();



        //    var video = await _unitOfWork.Repository<Videos>().GetByIdAsync(id);
        //    //if (video == null)
        //    //    return null;

        //    if (newImage != null)
        //    {
        //        // حذف القديمة
        //        if (!string.IsNullOrEmpty(category.ImagePath))
        //        {
        //            var publicId = _imageService.ExtractPublicIdFromUrl(category.ImagePath);
        //            _imageService.DeleteFromCloudinary(publicId);
        //        }

        //        category.ImagePath = await _imageService.UploadToCloudinary(newImage);
        //    }

        //    category.Name = categoryDto.Name;
        //    category.Description = categoryDto.Description;

        //    _unitOfWork.Repository<Categories>().Update(category);
        //    var status = await _unitOfWork.CompleteAsync();

        //    return status == 0 ? null : _mapper.Map<categoryDto>(category);


        //}












        public async Task<VideoDto> UpdateAsync (int id , CreateVideoDto videoDto)
        {
            var video = await _unitOfWork.Repository<Videos>().GetByIdAsync(id);
            if (video == null)
                return null;

            if (videoDto.video != null)
            {
                // حذف القديمة
                if (!string.IsNullOrEmpty(video.Url))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(video.Url);
                    _imageService.DeleteFromCloudinary(publicId);
                }

                video.Url = await _imageService.UploadToCloudinary(videoDto.video);
            }




            if (videoDto.IsDefault == true)
            {
                var current = await _ctx.Videos.Where(x => x.IsDefault).ToListAsync();
                foreach (var c in current) c.IsDefault = false;
            }
            await _unitOfWork.CompleteAsync();

            video.IsDefault = videoDto.IsDefault;
            _unitOfWork.Repository<Videos>().Update(video);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<VideoDto>(video);

        }











        public async Task<int> DeleteAsync(int id)
        {

            var video = await _unitOfWork.Repository<Videos>().GetByIdAsync(id);
            if (video == null)
                return 0;

            if (!string.IsNullOrEmpty(video.Url))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(video.Url);
                _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Videos>().Delete(video);
            return await _unitOfWork.CompleteAsync();

        }

        public async Task SetDefaultAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Videos>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Video not found");
            var current = await _ctx.Videos.Where(x => x.IsDefault && x.Id != id).ToListAsync();
            foreach (var c in current) c.IsDefault = false;
            await _unitOfWork.CompleteAsync();
            entity.IsDefault = true;
            await _unitOfWork.CompleteAsync();

        }

    }
}
