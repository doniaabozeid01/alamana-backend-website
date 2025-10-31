using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Category.Dtos;
using Microsoft.AspNetCore.Http;

namespace Alamana.Service.Category
{
    public interface ICategoryServices
    {
        Task<categoryDto> AddCategory(AddCategoryDto categoryDto, IFormFile image);
       
        Task<categoryDto> UpdateCategory(int id, AddCategoryDto categoryDto, IFormFile newImage);

        Task<categoryDto> GetCategoryById(int id);
        Task<IReadOnlyList<categoryDto>> GetAllCategories();
        //Task<int> DeleteCategory(int id);
        Task<int> DeleteCategory(int id);
        Task<CategoryWithProductsDto> GetCategoryByIdWithInclude(int id);
        //Task<IReadOnlyList<categoryDto>> GetAllCategoriesWithInclude();
    }
}
