using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            if (productDto.CountryId <= 0)
                throw new InvalidOperationException("CountryId is required.");

            var detailItems = ResolveDetailItemsForAdd(productDto);

            var product = new Products
            {
                NameEn = productDto.NameEn,
                NameAr = productDto.NameAr,
                DescriptionEn = productDto.DescriptionEn,
                DescriptionAr = productDto.DescriptionAr,
                CategoryId = productDto.CategoryId,
                Weight = productDto.Weight,
            };

            await _unitOfWork.Repository<Products>().AddAsync(product);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.Repository<CountryProducts>().AddAsync(new CountryProducts
            {
                ProductId = product.Id,
                CountryId = productDto.CountryId,
                Price = productDto.Price,
                Discount = productDto.Discount,
                IsNew = productDto.New,
                Stock = productDto.Stock,
            });
            await _unitOfWork.CompleteAsync();

            if (detailItems is { Count: > 0 })
                await ReplaceProductDetailEntriesAsync(product.Id, detailItems);

            var mediaRepo = _unitOfWork.Repository<ProductMedia>();

            if (productDto.Gallery is not null && productDto.Gallery.Count > 0)
            {
                foreach (var file in productDto.Gallery.Where(f => f != null && f.Length > 0))
                {
                    var url = await _imageService.UploadToCloudinary(file);
                    if (string.IsNullOrWhiteSpace(url))
                        continue;

                    await mediaRepo.AddAsync(new ProductMedia
                    {
                        ProductId = product.Id,
                        Type = DetectMediaType(file),
                        Url = url,
                    });
                }

                var status = await _unitOfWork.CompleteAsync();
                if (status == 0) return null;
            }

            var loaded = await _unitOfWork.Repository<Products>().Query()
                .Include(p => p.Category)
                .Include(p => p.Media)
                .Include(p => p.DetailEntries)
                .Include(p => p.CountryProducts)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            return loaded == null ? null : MapToProductDto(loaded, productDto.CountryId);
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

            IReadOnlyList<(string key, string value, int sort)>? parsedDetailsReplacement = null;
            var detailsTouched = false;
            if (HasMeaningfulDetailsJson(dto.DetailsJson))
            {
                if (TryParseDetailsJsonArray(dto.DetailsJson!, out var fromJson))
                {
                    detailsTouched = true;
                    parsedDetailsReplacement = fromJson;
                }
            }
            else if (dto.Details != null)
            {
                detailsTouched = true;
                parsedDetailsReplacement = NormalizeDetailsFromForm(dto.Details);
            }

            // 1) بيانات أساسية
            product.NameEn = dto.NameEn;
            product.NameAr = dto.NameAr;
            product.DescriptionEn = dto.DescriptionEn;
            product.DescriptionAr = dto.DescriptionAr;
            product.CategoryId = dto.CategoryId;
            product.Weight = dto.Weight;

            if (dto.CountryId > 0)
            {
                var countryProductRepo = _unitOfWork.Repository<CountryProducts>();
                var listing = await countryProductRepo.Query()
                    .FirstOrDefaultAsync(cp => cp.ProductId == id && cp.CountryId == dto.CountryId);

                if (listing == null)
                {
                    await countryProductRepo.AddAsync(new CountryProducts
                    {
                        ProductId = id,
                        CountryId = dto.CountryId,
                        Price = dto.Price,
                        Discount = dto.Discount,
                        IsNew = dto.New,
                        Stock = dto.Stock,
                    });
                }
                else
                {
                    listing.Price = dto.Price;
                    listing.Discount = dto.Discount;
                    listing.IsNew = dto.New;
                    listing.Stock = dto.Stock;
                    countryProductRepo.Update(listing);
                }
            }

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
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        // Skip failed uploads so Url never violates required constraint.
                        continue;
                    }

                    await mediaRepo.AddAsync(new ProductMedia
                    {
                        ProductId = product.Id,
                        Type = DetectMediaType(file),
                        Url = url,

                    });
                }
            }

            productRepo.Update(product);
            var status = await _unitOfWork.CompleteAsync();
            if (status == 0) return null;

            if (detailsTouched)
                await ReplaceProductDetailEntriesAsync(id, parsedDetailsReplacement!);

            var products = _unitOfWork.Repository<Products>();
            var updated = await products.Query()
                .Include(p => p.Category)
                .Include(p => p.Media)
                .Include(p => p.DetailEntries)
                .Include(p => p.CountryProducts)
                .FirstOrDefaultAsync(p => p.Id == id);

            return MapToProductDto(updated, dto.CountryId > 0 ? dto.CountryId : null);
        }





        //public async Task<ProductDto> GetProductByIdWithInclude(int id)
        //{
        //    var category = await _unitOfWork.Repository<Categories>().GetCategoryByIdAsync(id);
        //    var mappedCategory = _mapper.Map<categoryDto>(category);
        //    return mappedCategory;
        //}





        public async Task<IReadOnlyList<ProductDto>> GetAllProducts(int? categoryId, int countryId)
        {
            var products = await _unitOfWork.Repository<Products>().GetAllProductsAsync(categoryId, countryId);
            return products.Select(p => MapToProductDto(p, countryId)).ToList();
        }

        public async Task<IReadOnlyList<ProductDto>> GetRandomProducts(int countryId)
        {
            var products = await _unitOfWork.Repository<Products>().GetRandomProductsAsync(countryId);
            return products.Select(p => MapToProductDto(p, countryId)).ToList();
        }

        public async Task<IReadOnlyList<ProductDto>> GetNewProducts(int countryId)
        {
            var products = await _unitOfWork.Repository<Products>().GetNewProducts(countryId);
            return products.Select(p => MapToProductDto(p, countryId)).ToList();
        }

        public async Task<ProductDto?> GetHeroProductAsync(int countryId)
        {
            var listing = await _context.CountryProducts
                .Include(cp => cp.Product)
                    .ThenInclude(p => p.Media)
                .Include(cp => cp.Product)
                    .ThenInclude(p => p.Category)
                .Include(cp => cp.Product)
                    .ThenInclude(p => p.DetailEntries)
                .Where(cp => cp.CountryId == countryId && cp.IsHeroProduct)
                .OrderBy(cp => cp.Id)
                .FirstOrDefaultAsync();

            return listing?.Product == null ? null : MapToProductDto(listing.Product, countryId);
        }

        public async Task<ProductDto> GetProductById(int id, int? countryId = null)
        {
            var product = await _unitOfWork.Repository<Products>().GetProductByIdAsync(id);
            if (product == null)
                return null;

            if (countryId.HasValue && !product.CountryProducts.Any(cp => cp.CountryId == countryId.Value))
                return null;

            return MapToProductDto(product, countryId);
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

        private static IReadOnlyList<(string key, string value, int sort)>? ResolveDetailItemsForAdd(AddProductDto dto)
        {
            //if (HasMeaningfulDetailsJson(dto.DetailsJson) &&
            //    TryParseDetailsJsonArray(dto.DetailsJson!, out var fromJson))
            //{
            //    return fromJson.Count > 0 ? fromJson : null;
            //}

            var list = NormalizeDetailsFromForm(dto.Details);
            return list.Count > 0 ? list : null;
        }

        /// <summary>يتجاهل قيم Swagger الافتراضية زي "string".</summary>
        private static bool HasMeaningfulDetailsJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;
            return !string.Equals(json.Trim(), "string", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class DetailJsonRow
        {
            public string? Key { get; set; }
            public string? Value { get; set; }
            public int? Order { get; set; }
            public int? SortOrder { get; set; }
        }

        private static readonly JsonSerializerOptions DetailsJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        /// <returns>false لو JSON غير صالح (نُهمِل الحقل).</returns>
        private static bool TryParseDetailsJsonArray(string json, out List<(string key, string value, int sort)> result)
        {
            result = new List<(string, string, int)>();
            var normalized = ProductDetailsJsonNormalize.ForJsonParse(json);
            if (TryDeserializeDetailJsonRows(normalized, out result))
                return true;

            var recovered = ProductDetailsJsonNormalize.TryRecoverUtf8MisreadAsLatin1(normalized);
            if (string.IsNullOrEmpty(recovered))
                return false;

            normalized = ProductDetailsJsonNormalize.ForJsonParse(recovered);
            return TryDeserializeDetailJsonRows(normalized, out result);
        }

        private static bool TryDeserializeDetailJsonRows(string normalized, out List<(string key, string value, int sort)> result)
        {
            result = new List<(string, string, int)>();
            try
            {
                var rows = JsonSerializer.Deserialize<List<DetailJsonRow>>(normalized, DetailsJsonSerializerOptions);
                if (rows == null)
                    return true;

                for (var i = 0; i < rows.Count; i++)
                {
                    var r = rows[i];
                    if (r == null || string.IsNullOrWhiteSpace(r.Key))
                        continue;
                    var sort = r.Order ?? r.SortOrder ?? i;
                    result.Add((r.Key.Trim(), r.Value?.Trim() ?? string.Empty, sort));
                }

                return true;
            }
            catch (JsonException)
            {
                result = new List<(string, string, int)>();
                return false;
            }
        }

        private static List<(string key, string value, int sort)> NormalizeDetailsFromForm(List<ProductDetailFormItem>? details)
        {
            var result = new List<(string, string, int)>();
            if (details == null) return result;
            for (var i = 0; i < details.Count; i++)
            {
                var d = details[i];
                if (d == null || string.IsNullOrWhiteSpace(d.Key))
                    continue;
                var sort = d.SortOrder ?? i;
                result.Add((d.Key.Trim(), d.Value?.Trim() ?? string.Empty, sort));
            }
            return result;
        }

        private async Task ReplaceProductDetailEntriesAsync(int productId, IReadOnlyList<(string key, string value, int sort)> items)
        {
            var detailRepo = _unitOfWork.Repository<ProductDetailEntry>();
            var existing = await detailRepo.Query().Where(x => x.ProductId == productId).ToListAsync();
            foreach (var e in existing)
                detailRepo.Delete(e);
            await _unitOfWork.CompleteAsync();

            foreach (var (key, value, sort) in items)
            {
                await detailRepo.AddAsync(new ProductDetailEntry
                {
                    ProductId = productId,
                    EntryKey = key,
                    EntryValue = value,
                    SortOrder = sort
                });
            }

            await _unitOfWork.CompleteAsync();
        }

        private decimal priceAfterDiscount(decimal price, decimal discount)
        {
            var result = 0m;
            result = price - (price * (discount / 100m));
            return result;

        }
















        public async Task<IReadOnlyList<ProductDto>> GetTopBestSellersAsync(int take, int countryId)
        {
            var topSales = await _context.OrderItem
                .Where(oi => oi.Order.CountryId == countryId)
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

            var topProducts = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Media)
                .Include(p => p.DetailEntries)
                .Include(p => p.CountryProducts)
                .Where(p => topIds.Contains(p.Id))
                .Where(p => p.CountryProducts.Any(cp => cp.CountryId == countryId))
                .ToListAsync();

            topProducts = topProducts
                .OrderBy(p => topIds.IndexOf(p.Id))
                .ToList();

            var remaining = take - topProducts.Count;
            if (remaining > 0)
            {
                var filler = await _context.Product
                    .Include(p => p.Category)
                    .Include(p => p.Media)
                    .Include(p => p.DetailEntries)
                    .Include(p => p.CountryProducts)
                    .Where(p => !topIds.Contains(p.Id))
                    .Where(p => p.CountryProducts.Any(cp => cp.CountryId == countryId))
                    .OrderByDescending(p => p.Id)
                    .Take(remaining)
                    .ToListAsync();

                topProducts.AddRange(filler);
            }

            return topProducts.Select(p => MapToProductDto(p, countryId)).ToList();
        }

        private ProductDto MapToProductDto(Products p, int? countryId)
        {
            var price = ProductCountryPricing.GetPrice(p, countryId);
            var discount = ProductCountryPricing.GetDiscount(p, countryId);
            var isNew = ProductCountryPricing.GetIsNew(p, countryId);

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
                New = isNew,
                priceAfterDiscount = priceAfterDiscount(price, discount),
                category = p.Category == null ? null : new productCategoryDto
                {
                    Id = p.Category.Id,
                    NameEn = p.Category.NameEn,
                    NameAr = p.Category.NameAr,
                    DescriptionEn = p.Category.DescriptionEn,
                    DescriptionAr = p.Category.DescriptionAr
                },
                GalleryUrls = p.Media?
                    .Select(m => new mediaDto { Url = m.Url, Type = m.Type })
                    .ToList() ?? new List<mediaDto>(),
                Details = p.DetailEntries?
                    .OrderBy(e => e.SortOrder)
                    .ThenBy(e => e.Id)
                    .Select(e => new ProductDetailEntryDto
                    {
                        Id = e.Id,
                        Key = e.EntryKey,
                        Value = e.EntryValue,
                        SortOrder = e.SortOrder
                    })
                    .ToList() ?? new List<ProductDetailEntryDto>()
            };
        }






    }
}
