using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Category.Dtos;
using Alamana.Service.Product;
using Alamana.Service.Product.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Service.Category
{
    public class CategoryServices : ICategoryServices
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISaveAndDeleteImageService _imageService;

        public CategoryServices(IUnitOfWork unitOfWork, IMapper mapper, ISaveAndDeleteImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;

        }




        public async Task<categoryDto> AddCategory(AddCategoryDto categoryDto)
        {
            var imageUrl = categoryDto.Image != null ? await _imageService.UploadToCloudinary(categoryDto.Image) : null;
            var mobileUrl = categoryDto.MobileImage != null ? await _imageService.UploadToCloudinary(categoryDto.MobileImage) : null;

            var category = new Categories
            {
                NameEn = categoryDto.NameEn,
                NameAr = categoryDto.NameAr,
                DescriptionEn = categoryDto.DescriptionEn,
                DescriptionAr = categoryDto.DescriptionAr,
                ImagePath = imageUrl,
                MobileImagePath = mobileUrl
            };

            await _unitOfWork.Repository<Categories>().AddAsync(category);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<categoryDto>(category);
        }






        public async Task<categoryDto> UpdateCategory(int id, AddCategoryDto categoryDto)
        {
            var category = await _unitOfWork.Repository<Categories>().GetByIdAsync(id);
            if (category == null)
                return null;

            if (categoryDto.Image != null)
            {
                if (!string.IsNullOrEmpty(category.ImagePath))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(category.ImagePath);
                    if (!string.IsNullOrEmpty(publicId))
                        _imageService.DeleteFromCloudinary(publicId);
                }

                category.ImagePath = await _imageService.UploadToCloudinary(categoryDto.Image);
            }

            if (categoryDto.MobileImage != null)
            {
                if (!string.IsNullOrEmpty(category.MobileImagePath))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(category.MobileImagePath);
                    if (!string.IsNullOrEmpty(publicId))
                        _imageService.DeleteFromCloudinary(publicId);
                }

                category.MobileImagePath = await _imageService.UploadToCloudinary(categoryDto.MobileImage);
            }

            category.NameEn = categoryDto.NameEn;
            category.NameAr = categoryDto.NameAr;
            category.DescriptionEn = categoryDto.DescriptionEn;
            category.DescriptionAr = categoryDto.DescriptionAr;

            _unitOfWork.Repository<Categories>().Update(category);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<categoryDto>(category);

        }






        public async Task<CategoryWithProductsDto> GetCategoryByIdWithInclude(int id, int countryId)
        {
            var category = await _unitOfWork.Repository<Categories>().Query()
                .Include(c => c.CountryCategories)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Media)
                .Include(c => c.Products)
                    .ThenInclude(p => p.DetailEntries)
                .Include(c => c.Products)
                    .ThenInclude(p => p.CountryProducts)
                .FirstOrDefaultAsync(c => c.Id == id && c.CountryCategories.Any(cc => cc.CountryId == countryId));

            if (category == null)
                return null;

            var productsInCountry = category.Products?
                .Where(p => p.CountryProducts.Any(cp => cp.CountryId == countryId))
                .ToList() ?? new List<Products>();

            return new CategoryWithProductsDto
            {
                CategoryId = category.Id,
                CategoryNameEn = category.NameEn,
                CategoryNameAr = category.NameAr,
                Products = productsInCountry.Select(p =>
                {
                    var price = ProductCountryPricing.GetPrice(p, countryId);
                    var discount = ProductCountryPricing.GetDiscount(p, countryId);
                    return new ProductDto
                    {
                        Id = p.Id,
                        NameEn = p.NameEn,
                        NameAr = p.NameAr,
                        Price = price,
                        Weight = p.Weight,
                        DescriptionEn = p.DescriptionEn,
                        DescriptionAr = p.DescriptionAr,
                        Discount = discount,
                        New = ProductCountryPricing.GetIsNew(p, countryId),
                        priceAfterDiscount = price - (price * (discount / 100m)),
                        GalleryUrls = p.Media.Select(m => new mediaDto
                        {
                            Url = m.Url,
                            Type = m.Type
                        }).ToList(),
                        Details = p.DetailEntries
                            .OrderBy(e => e.SortOrder)
                            .ThenBy(e => e.Id)
                            .Select(e => new ProductDetailEntryDto
                            {
                                Id = e.Id,
                                KeyEn = e.EntryKeyEn,
                                KeyAr = e.EntryKeyAr,
                                ValueEn = e.EntryValueEn,
                                ValueAr = e.EntryValueAr,
                                SortOrder = e.SortOrder
                            })
                            .ToList()
                    };
                }).ToList()
            };
        }






        public async Task<IReadOnlyList<categoryDto>> GetAllCategories(int countryId)
        {
            var category = await _unitOfWork.Repository<Categories>().Query()
                .Where(c => c.CountryCategories.Any(cc => cc.CountryId == countryId))
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<categoryDto>>(category);
        }





        public async Task<categoryDto> GetCategoryById(int id, int? countryId = null)
        {
            var query = _unitOfWork.Repository<Categories>().Query().Where(c => c.Id == id);
            if (countryId.HasValue)
                query = query.Where(c => c.CountryCategories.Any(cc => cc.CountryId == countryId.Value));

            var category = await query.FirstOrDefaultAsync();
            return category == null ? null : _mapper.Map<categoryDto>(category);
        }




        public async Task<int> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repository<Categories>().GetByIdAsync(id);
            if (category == null)
                return 0;

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(category.ImagePath);
                if (!string.IsNullOrEmpty(publicId))
                    _imageService.DeleteFromCloudinary(publicId);
            }

            if (!string.IsNullOrEmpty(category.MobileImagePath))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(category.MobileImagePath);
                if (!string.IsNullOrEmpty(publicId))
                    _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Categories>().Delete(category);
            return await _unitOfWork.CompleteAsync();
        }





    }
}
