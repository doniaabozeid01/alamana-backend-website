using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Advertisment.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
                TitleEn = imageDto.TitleEn,
                TitleAr = imageDto.TitleAr,
                DescriptionEn = imageDto.DescriptionEn,
                DescriptionAr = imageDto.DescriptionAr,
            };

            await _unitOfWork.Repository<Advertisements>().AddAsync(img);
            var status = await _unitOfWork.CompleteAsync();

            if (status == 0)
                return null;

            await SyncAdvertisementProductsAsync(img.Id, imageDto.ProductIds);

            return await GetAdvertisementDtoByIdAsync(img.Id);
        }

        public async Task<AdvertiseDto> UpdateAdvertise(int id, AddAdvertise imageDto)
        {
            var img = await _unitOfWork.Repository<Advertisements>().GetByIdAsync(id);
            if (img == null)
                return null;

            if (imageDto.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(img.ImageUrl))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(img.ImageUrl);
                    if (!string.IsNullOrEmpty(publicId))
                        _imageService.DeleteFromCloudinary(publicId);
                }

                img.ImageUrl = await _imageService.UploadToCloudinary(imageDto.ImageUrl);
            }

            img.TitleEn = imageDto.TitleEn;
            img.TitleAr = imageDto.TitleAr;
            img.DescriptionEn = imageDto.DescriptionEn;
            img.DescriptionAr = imageDto.DescriptionAr;

            _unitOfWork.Repository<Advertisements>().Update(img);
            var status = await _unitOfWork.CompleteAsync();

            if (status == 0)
                return null;

            if (imageDto.ProductIds != null)
                await SyncAdvertisementProductsAsync(id, imageDto.ProductIds);

            return await GetAdvertisementDtoByIdAsync(id);
        }

        public async Task<IReadOnlyList<AdvertiseDto>> GetAllAdvertisements(int? countryId = null)
        {
            var query = _unitOfWork.Repository<Advertisements>()
                .Query()
                .Include(a => a.AdvertisementProducts)
                .AsQueryable();

            if (countryId.HasValue && countryId.Value > 0)
            {
                query = query.Where(a => a.CountryAdvertisements.Any(ca => ca.CountryId == countryId.Value));
            }

            var imgs = await query.OrderByDescending(a => a.Id).ToListAsync();
            return _mapper.Map<IReadOnlyList<AdvertiseDto>>(imgs);
        }

        public async Task<AdvertiseDto> GetAdvertisementById(int id)
        {
            return await GetAdvertisementDtoByIdAsync(id);
        }

        public async Task<int> DeleteAdvertisement(int id)
        {
            var img = await _unitOfWork.Repository<Advertisements>().GetByIdAsync(id);
            if (img == null)
                return 0;

            if (!string.IsNullOrEmpty(img.ImageUrl))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(img.ImageUrl);
                if (!string.IsNullOrEmpty(publicId))
                    _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Advertisements>().Delete(img);
            return await _unitOfWork.CompleteAsync();
        }

        private async Task<AdvertiseDto?> GetAdvertisementDtoByIdAsync(int id)
        {
            var img = await _unitOfWork.Repository<Advertisements>()
                .Query()
                .Include(a => a.AdvertisementProducts)
                .FirstOrDefaultAsync(a => a.Id == id);

            return img == null ? null : _mapper.Map<AdvertiseDto>(img);
        }

        private async Task SyncAdvertisementProductsAsync(int advertisementId, IList<int>? productIds)
        {
            var linkRepo = _unitOfWork.Repository<AdvertisementProduct>();

            var existing = await linkRepo.Query()
                .Where(x => x.AdvertisementId == advertisementId)
                .ToListAsync();

            foreach (var row in existing)
                linkRepo.Delete(row);

            await _unitOfWork.CompleteAsync();

            var ids = (productIds ?? Array.Empty<int>())
                .Where(pid => pid > 0)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return;

            var validIds = await _unitOfWork.Repository<Products>()
                .Query()
                .Where(p => ids.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var pid in validIds)
            {
                await linkRepo.AddAsync(new AdvertisementProduct
                {
                    AdvertisementId = advertisementId,
                    ProductId = pid
                });
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
