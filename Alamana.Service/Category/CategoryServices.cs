using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Category.Dtos;
using Alamana.Service.Product.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;
using Microsoft.AspNetCore.Http;

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




        public async Task<categoryDto> AddCategory(AddCategoryDto categoryDto, IFormFile image)
        {
            var imageUrl = image != null ? await _imageService.UploadToCloudinary(image) : null;

            var category = new Categories
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ImagePath = imageUrl
            };

            await _unitOfWork.Repository<Categories>().AddAsync(category);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<categoryDto>(category);
        }






        public async Task<categoryDto> UpdateCategory(int id, AddCategoryDto categoryDto, IFormFile newImage)
        {
            var category = await _unitOfWork.Repository<Categories>().GetByIdAsync(id);
            if (category == null)
                return null;

            if (newImage != null)
            {
                // حذف القديمة
                if (!string.IsNullOrEmpty(category.ImagePath))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(category.ImagePath);
                    _imageService.DeleteFromCloudinary(publicId);
                }

                category.ImagePath = await _imageService.UploadToCloudinary(newImage);
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            _unitOfWork.Repository<Categories>().Update(category);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<categoryDto>(category);
        }






        public async Task<CategoryWithProductsDto> GetCategoryByIdWithInclude(int id)
        {
            var category = await _unitOfWork.Repository<Categories>().GetCategoryByIdAsync(id);

            if (category == null)
                return null;

            return new CategoryWithProductsDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Products = category.Products?.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Weight = p.Weight,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    ImagePathCover = p.ImagePathCover
                }).ToList()
            };
        }






        public async Task<IReadOnlyList<categoryDto>> GetAllCategories()
        {
            var category = await _unitOfWork.Repository<Categories>().GetAllAsync();
            var mappedCategory = _mapper.Map<IReadOnlyList<categoryDto>>(category);
            return mappedCategory;
        }





        public async Task<categoryDto> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Repository<Categories>().GetByIdAsync(id);
            var mappedCategory = _mapper.Map<categoryDto>(category);
            return mappedCategory;
        }




        public async Task<int> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repository<Categories>().GetByIdAsync(id);
            if (category == null)
                return 0;

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(category.ImagePath);
                _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Categories>().Delete(category);
            return await _unitOfWork.CompleteAsync();
        }





    }
}
