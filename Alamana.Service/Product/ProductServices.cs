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

namespace Alamana.Service.Product
{
    public class ProductServices : IProductServices
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISaveAndDeleteImageService _imageService;

        public ProductServices(IUnitOfWork unitOfWork, IMapper mapper, ISaveAndDeleteImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;

        }




        public async Task<ProductDto> AddProduct(AddProductDto productDto, IFormFile image)
        {
            var imageUrl = image != null ? await _imageService.UploadToCloudinary(image) : null;

            var product = new Products
            {
                Name = productDto.Name,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                Weight = productDto.Weight,
                ImagePathCover = imageUrl
            };

            await _unitOfWork.Repository<Products>().AddAsync(product);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<ProductDto>(product);
        }






        public async Task<ProductDto> UpdateProduct(int id, AddProductDto productDto, IFormFile newImage)
        {
            var product = await _unitOfWork.Repository<Products>().GetByIdAsync(id);
            if (product == null)
                return null;

            if (newImage != null)
            {
                // حذف القديمة
                if (!string.IsNullOrEmpty(product.ImagePathCover))
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(product.ImagePathCover);
                    _imageService.DeleteFromCloudinary(publicId);
                }

                product.ImagePathCover = await _imageService.UploadToCloudinary(newImage);
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.CategoryId = productDto.CategoryId;
            product.Price = productDto.Price;
            product.Weight = productDto.Weight;


            _unitOfWork.Repository<Products>().Update(product);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<ProductDto>(product);
        }






        //public async Task<ProductDto> GetProductByIdWithInclude(int id)
        //{
        //    var category = await _unitOfWork.Repository<Categories>().GetCategoryByIdAsync(id);
        //    var mappedCategory = _mapper.Map<categoryDto>(category);
        //    return mappedCategory;
        //}





        public async Task<IReadOnlyList<ProductDto>> GetAllProducts()
        {
            var products = await _unitOfWork.Repository<Products>().GetAllAsync();
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return mappedProducts;
        }





        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _unitOfWork.Repository<Products>().GetByIdAsync(id);
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return mappedProduct;
        }




        public async Task<int> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Products>().GetByIdAsync(id);
            if (product == null)
                return 0;

            if (!string.IsNullOrEmpty(product.ImagePathCover))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(product.ImagePathCover);
                _imageService.DeleteFromCloudinary(publicId);
            }

            _unitOfWork.Repository<Products>().Delete(product);
            return await _unitOfWork.CompleteAsync();
        }







    }
}
