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

        Task<ProductDto> GetProductById(int id);
        Task<IReadOnlyList<ProductDto>> GetAllProducts();
        Task<IReadOnlyList<ProductDto>> GetRandomProducts();
        //Task<int> DeleteCategory(int id);
        Task<bool> DeleteProduct(int id);
        //Task<ProductDto> GetCategoryByIdWithInclude(int id);
    }
}
