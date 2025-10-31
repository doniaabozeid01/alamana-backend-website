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
        Task<ProductDto> AddProduct(AddProductDto productDto, IFormFile image);

        Task<ProductDto> UpdateProduct(int id, AddProductDto productDto, IFormFile newImage);

        Task<ProductDto> GetProductById(int id);
        Task<IReadOnlyList<ProductDto>> GetAllProducts();
        //Task<int> DeleteCategory(int id);
        Task<int> DeleteProduct(int id);
        //Task<ProductDto> GetCategoryByIdWithInclude(int id);
    }
}
