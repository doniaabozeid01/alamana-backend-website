using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.Category.Dtos;
namespace Alamana.Service.Category
{
    public interface ICategoryServices
    {
        Task<categoryDto> AddCategory(AddCategoryDto categoryDto);

        Task<categoryDto> UpdateCategory(int id, AddCategoryDto categoryDto);

        Task<categoryDto> GetCategoryById(int id);
        Task<IReadOnlyList<categoryDto>> GetAllCategories();
        //Task<int> DeleteCategory(int id);
        Task<int> DeleteCategory(int id);
        Task<CategoryWithProductsDto> GetCategoryByIdWithInclude(int id);
        //Task<IReadOnlyList<categoryDto>> GetAllCategoriesWithInclude();
    }
}
