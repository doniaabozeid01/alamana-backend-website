using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Advertisment.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;

namespace Alamana.Service.Advertisment
{
    public class AdvertiseService : IAdvertiseService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISaveAndDeleteImageService _imageService;


        public AdvertiseService(IUnitOfWork unitOfWork, IMapper mapper, ISaveAndDeleteImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<AdvertiseDto> AddAdvertise(AddAdvertise imageDto)
        {
            if (imageDto.ImageUrl == null || imageDto.ImageUrl.Length == 0)
                return null;

            string imageUrl = await _imageService.UploadToCloudinary(imageDto.ImageUrl);

            var img = new Advertisements
            {
                ImageUrl = imageUrl,
                Title = imageDto.Title,
                //TitleAr = imageDto.TitleAr,
                Description = imageDto.Description,
                //DescriptionAr = imageDto.DescriptionAr,
            };

            await _unitOfWork.Repository<Advertisements>().AddAsync(img);
            var status = await _unitOfWork.CompleteAsync();

            if (status == 0)
                return null;

            return _mapper.Map<AdvertiseDto>(img);
        }




        public async Task<AdvertiseDto> UpdateAdvertise(int id, AddAdvertise imageDto)
        {
            var img = await _unitOfWork.Repository<Advertisements>().GetByIdAsync(id);
            if (img == null)
                return null;

            if (imageDto.ImageUrl != null)
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(img.ImageUrl);
                _imageService.DeleteFromCloudinary(publicId); // حذف القديمة
                img.ImageUrl = await _imageService.UploadToCloudinary(imageDto.ImageUrl); // رفع الجديدة
            }

            imageDto.Title = imageDto.Title;
            //imageDto.TitleAr = imageDto.TitleAr;
            imageDto.Description = imageDto.Description;
            //imageDto.DescriptionAr = imageDto.DescriptionAr;

            _unitOfWork.Repository<Advertisements>().Update(img);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<AdvertiseDto>(img);
        }


        public async Task<IReadOnlyList<AdvertiseDto>> GetAllAdvertisements()
        {
            var imgs = await _unitOfWork.Repository<Advertisements>().GetAllAsync();
            return _mapper.Map<IReadOnlyList<AdvertiseDto>>(imgs);
        }




        public async Task<AdvertiseDto> GetAdvertisementById(int id)
        {
            var img = await _unitOfWork.Repository<Advertisements>().GetByIdAsync(id);
            return _mapper.Map<AdvertiseDto>(img);
        }

        public async Task<int> DeleteAdvertisement(int id)
        {
            var img = await _unitOfWork.Repository<Advertisements>().GetByIdAsync(id);
            if (img == null)
                return 0;

            if (!string.IsNullOrEmpty(img.ImageUrl))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(img.ImageUrl);
                _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Advertisements>().Delete(img);
            return await _unitOfWork.CompleteAsync();
        }

    }

}
