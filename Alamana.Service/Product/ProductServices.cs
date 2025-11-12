using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
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
        private readonly AlamanaBbContext _context;

        public ProductServices(IUnitOfWork unitOfWork, IMapper mapper, ISaveAndDeleteImageService imageService , AlamanaBbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
            _context = context;
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
                New = productDto.New,
                Weight = productDto.Weight,
                Discount = productDto.Discount,
            };

            await _unitOfWork.Repository<Products>().AddAsync(product);
            await _unitOfWork.CompleteAsync(); // للحصول على product.Id

            var mediaRepo = _unitOfWork.Repository<ProductMedia>();
            int sort = 0;


            // 3) رفع الجاليري (اختياري)
            // 3) رفع الجاليري (اختياري)
            if (productDto.Gallery is not null && productDto.Gallery.Count > 0)
            {
                foreach (var file in productDto.Gallery.Where(f => f != null && f.Length > 0))
                {
                    var url = await _imageService.UploadToCloudinary(file);

                    var media = new ProductMedia
                    {
                        ProductId = product.Id,
                        Type = DetectMediaType(file), // 👈 هنا بنحدّد Image أو Video
                        Url = url,
                        // (اختياري) خزين معلومات مفيدة للديبَغينغ
                        //MimeType = file.ContentType,
                        //FileName = file.FileName
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
                Discount = product.Discount,
                priceAfterDiscount = priceAfterDiscount(product.Price, product.Discount),

                category = new productCategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                },
                New = product.New,
                // أول صورة فقط


                // باقي الصور (قائمة)
                // قائمة الصور والفيديوهات مع النوع
    //            GalleryUrls = product.Media
    //.Select(m => new
    //{
    //    Url = m.Url,
    //    Type = m.Type.ToString() // يحوّل enum إلى "Image" أو "Video"
    //})
    //.ToList();

            };

            return dto;
        }

        private static MediaType DetectMediaType(IFormFile file)
        {
            // 1) من الـ MIME
            if (!string.IsNullOrWhiteSpace(file.ContentType))
            {
                if (file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                    return MediaType.Video;

                if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return MediaType.Image;
            }

            // 2) من الامتداد (fallback)
            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            // اشهر فيديوهات
            var videoExts = new HashSet<string> { ".mp4", ".mov", ".mkv", ".webm", ".avi" };
            if (!string.IsNullOrEmpty(ext) && videoExts.Contains(ext))
                return MediaType.Video;

            // اشهر صور
            var imageExts = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            if (!string.IsNullOrEmpty(ext) && imageExts.Contains(ext))
                return MediaType.Image;

            // افتراضيًا اعتبرها صورة
            return MediaType.Image;
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
            product.New = dto.New;
            product.Discount = dto.Discount;

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
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Weight = p.Weight,
                Description = p.Description,
                Discount = p.Discount,
                New = p.New,
                priceAfterDiscount = priceAfterDiscount(p.Price, p.Discount),

                category = p.Category == null ? null : new productCategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                },
                // باقي الصور
                GalleryUrls = p.Media
                .Select(m => new mediaDto
                {
                    Url = m.Url,
                    Type = m.Type
                })
                .ToList()


            })
                .ToList();

            return result;
        }





        public async Task<IReadOnlyList<ProductDto>> GetRandomProducts()
        {
            var products = await _unitOfWork.Repository<Products>().GetRandomProductsAsync();
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Weight = p.Weight,
                Description = p.Description,
                Discount= p.Discount,
                New = p.New,
                priceAfterDiscount = priceAfterDiscount(p.Price, p.Discount),
                category = p.Category == null ? null : new productCategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                },
                // باقي الصور
                GalleryUrls = p.Media
                .Select(m => new mediaDto
                {
                    Url = m.Url,
                    Type = m.Type
                })
                .ToList()


            })
                .ToList(); return result;
        }







        public async Task<IReadOnlyList<ProductDto>> GetNewProducts()
        {
            var products = await _unitOfWork.Repository<Products>().GetNewProducts();
            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Weight = p.Weight,
                Description = p.Description,
                New = p.New,
                Discount = p.Discount,
                priceAfterDiscount = priceAfterDiscount(p.Price, p.Discount),
                category = p.Category == null ? null : new productCategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                },
                // باقي الصور
                GalleryUrls = p.Media
                .Select(m => new mediaDto
                {
                    Url = m.Url,
                    Type = m.Type
                })
                .ToList()


            })
                .ToList(); 
            return result;
        }



        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _unitOfWork.Repository<Products>().GetProductByIdAsync(id);
            //var mappedProduct = _mapper.Map<ProductDto>(product);
            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Weight = product.Weight,
                Description = product.Description,
                New = product.New,
                Discount = product.Discount,
                priceAfterDiscount = priceAfterDiscount(product.Price, product.Discount),
                category = product.Category == null ? null : new productCategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                },
                // باقي الصور
                GalleryUrls = product.Media
    .Select(m => new mediaDto
    {
        Url = m.Url,
        Type = m.Type
    })
    .ToList()
            };

            
            return result;
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




        private decimal priceAfterDiscount(decimal price, decimal discount)
        {
            var result = 0m;
            result = price - (price * (discount / 100m));
            return result;

        }
















        public async Task<IReadOnlyList<ProductDto>> GetTopBestSellersAsync(int take = 5)
        {
            // 1️⃣ نجيب إجمالي المبيعات حسب المنتج
            var topSales = await _context.OrderItem
                .GroupBy(o => o.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(take)
                .ToListAsync();

            var topIds = topSales.Select(x => x.ProductId).ToList();

            // 2️⃣ نجيب المنتجات دي
            var topProducts = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Media)
                .Where(p => topIds.Contains(p.Id))
                .ToListAsync();

            // 3️⃣ نحافظ على الترتيب حسب المبيعات
            topProducts = topProducts
                .OrderBy(p => topIds.IndexOf(p.Id))
                .ToList();

            // 4️⃣ لو أقل من المطلوب، نكمّل من باقي المنتجات
            var remaining = take - topProducts.Count;
            if (remaining > 0)
            {
                var filler = await _context.Product
                    .Include(p => p.Category)
                    .Include(p => p.Media)
                    .Where(p => !topIds.Contains(p.Id))
                    .OrderByDescending(p => p.Id) // أو CreatedAt
                    .Take(remaining)
                    .ToListAsync();

                topProducts.AddRange(filler);
            }

            // 5️⃣ نحولهم إلى DTO
            var result = topProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                New = p.New,                      // لو عندك عمود IsNew في الـ Entity
                Discount = p.Discount,
                priceAfterDiscount = p.Price - (p.Price * (p.Discount / 100)),
                Weight = p.Weight,
                Description = p.Description,
                GalleryUrls = p.Media
                    .Select(m => new mediaDto
                    {
                        Url = m.Url,
                        Type = m.Type
                    })
                    .ToList(),
                category = new productCategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                }
            }).ToList();

            return result;
        }






    }
}
