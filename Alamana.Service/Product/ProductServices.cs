using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Data.Enums;
using Alamana.Repository.Interfaces;
using Alamana.Repository.Repositories;
using Alamana.Service.Category.Dtos;
using Alamana.Service.Product.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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




        //public async Task<ProductDto> AddProduct(AddProductDto productDto, IFormFile image)
        //{
        //    var imageUrl = image != null ? await _imageService.UploadToCloudinary(image) : null;

        //    var product = new Products
        //    {
        //        Name = productDto.Name,
        //        Description = productDto.Description,
        //        CategoryId = productDto.CategoryId,
        //        Price = productDto.Price,
        //        Weight = productDto.Weight,
        //        ImagePathCover = imageUrl
        //    };

        //    await _unitOfWork.Repository<Products>().AddAsync(product);
        //    var status = await _unitOfWork.CompleteAsync();

        //    return status == 0 ? null : _mapper.Map<ProductDto>(product);
        //}








        public async Task<ProductDto> AddProduct(AddProductDto productDto)
        {
            // 1) إنشاء المنتج (بدون صور)
            var product = new Products
            {
                Name = productDto.Name,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                Weight = productDto.Weight
            };

            await _unitOfWork.Repository<Products>().AddAsync(product);
            await _unitOfWork.CompleteAsync(); // للحصول على product.Id

            var mediaRepo = _unitOfWork.Repository<ProductMedia>();
            int sort = 0;


            // 3) رفع الجاليري (اختياري)
            if (productDto.Gallery is not null && productDto.Gallery.Count > 0)
            {
                foreach (var file in productDto.Gallery.Where(f => f != null && f.Length > 0))
                {
                    // (اختياري) تحقق من النوع: image/*
                    var url = await _imageService.UploadToCloudinary(file);
                    var media = new ProductMedia
                    {
                        ProductId = product.Id,
                        Type = MediaType.Image,
                        Url = url,

                    };
                    await mediaRepo.AddAsync(media);
                }
            }

            var status = await _unitOfWork.CompleteAsync();
            if (status == 0) return null;

            // 4) ارجعي المنتج مع الوسائط (Load ثم Map)
            var productWithMedia = await _unitOfWork.Repository<Products>().GetAllProductsAsync();


            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Weight = product.Weight,
                Description = product.Description,
                CategoryId = product.CategoryId,
                // أول صورة فقط
   

                // باقي الصور (قائمة)
                GalleryUrls = product.Media
                        .Where(m => m.Type == MediaType.Image)
                        .Select(m => m.Url)
                        .ToList()
            };

            return dto;
        }












        public async Task<ProductDto> UpdateProduct(int id, UpdateProductDto dto)
        {
            var productRepo = _unitOfWork.Repository<Products>();
            var mediaRepo = _unitOfWork.Repository<ProductMedia>();

            var product = await productRepo.Query()
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            // 1) بيانات أساسية
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;
            product.Weight = dto.Weight;

            // helper: آخر ترتيب
            //int maxSort = product.Media.Any() ? product.Media.Max(m => m.SortOrder) : 0;

            // 2) حذف عناصر مطلوبة
            if (dto.RemoveMediaIds?.Count > 0)
            {
                var toRemove = product.Media.Where(m => dto.RemoveMediaIds.Contains(m.Id)).ToList();
                foreach (var m in toRemove)
                {
                    var publicId = _imageService.ExtractPublicIdFromUrl(m.Url);
                    if (!string.IsNullOrEmpty(publicId))
                        _imageService.DeleteFromCloudinary(publicId);

                    mediaRepo.Delete(m);
                }
            }





            // 5) إضافة جاليري جديد
            if (dto.NewGallery?.Count > 0)
            {
                foreach (var file in dto.NewGallery.Where(f => f != null && f.Length > 0))
                {
                    var url = await _imageService.UploadToCloudinary(file);
                    await mediaRepo.AddAsync(new ProductMedia
                    {
                        ProductId = product.Id,
                        Type = MediaType.Image,
                        Url = url,

                    });
                }
            }

            productRepo.Update(product);
            var status = await _unitOfWork.CompleteAsync();
            if (status == 0) return null;

            // رجّعي المنتج مع الوسائط مرتبة

            var products  = _unitOfWork.Repository<Products>();
            var updated = await products.Query()
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<ProductDto>(updated);
        }





        //public async Task<ProductDto> GetProductByIdWithInclude(int id)
        //{
        //    var category = await _unitOfWork.Repository<Categories>().GetCategoryByIdAsync(id);
        //    var mappedCategory = _mapper.Map<categoryDto>(category);
        //    return mappedCategory;
        //}





        public async Task<IReadOnlyList<ProductDto>> GetAllProducts()
        {
            var products = await _unitOfWork.Repository<Products>().GetAllProductsAsync();
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return mappedProducts;
        }





        public async Task<IReadOnlyList<ProductDto>> GetRandomProducts()
        {
            var products = await _unitOfWork.Repository<Products>().GetRandomProductsAsync();
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return mappedProducts;
        }







        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _unitOfWork.Repository<Products>().GetByIdAsync(id);
            var mappedProduct = _mapper.Map<ProductDto>(product);
            return mappedProduct;
        }





public async Task<bool> DeleteProduct(int id)
    {
        // هات المنتج مع الوسائط
        var productRepo = _unitOfWork.Repository<Products>();
        var mediaRepo = _unitOfWork.Repository<ProductMedia>();

        var product = await productRepo.Query()
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return false;

        // احذف صور Cloudinary أولاً (لو عندك فيديوهات برابط خارجي تجاهلها)
        foreach (var m in product.Media)
        {
            if (!string.IsNullOrWhiteSpace(m.Url))
            {
                var publicId = _imageService.ExtractPublicIdFromUrl(m.Url);
                if (!string.IsNullOrEmpty(publicId))
                {
                    try { _imageService.DeleteFromCloudinary(publicId); }
                    catch { /* ممكن تعملي log وتحبي تكمّلي */ }
                }
            }
        }

        // لو عندك Cascade من Product -> ProductMedia فمش لازم تحذفي Media يدوي
        // غير كده احذفيها صراحة:
        // foreach (var m in product.Media) mediaRepo.Delete(m);

        productRepo.Delete(product);

        var status = await _unitOfWork.CompleteAsync();
        return status > 0;
    }







}
}
