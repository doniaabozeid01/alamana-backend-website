using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Category.Dtos;
using Alamana.Service.Product.Dtos;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Product
{
    public interface IProductServices
    {
        Task<ProductDto> AddProduct(AddProductDto productDto);

        Task<ProductDto> UpdateProduct(int id, UpdateProductDto dto);

        Task<ProductDto> GetProductById(int id, int? countryId = null);
        Task<IReadOnlyList<ProductDto>> GetAllProducts(int? categoryId, int countryId);
        Task<IReadOnlyList<ProductDto>> GetRandomProducts(int countryId);
        Task<IReadOnlyList<ProductDto>> GetNewProducts(int countryId);
        Task<ProductDto?> GetHeroProductAsync(int countryId);

        Task<bool> DeleteProduct(int id);
        Task<IReadOnlyList<ProductDto>> GetTopBestSellersAsync(int take, int countryId);
    }
}
