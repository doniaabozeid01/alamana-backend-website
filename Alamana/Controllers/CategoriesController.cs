using Alamana.Service.Category.Dtos;
using Alamana.Service.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {



        private readonly ICategoryServices _categoryServices;

        public CategoriesController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }




        [HttpPost("AddCategory")]
        public async Task<ActionResult<categoryDto>> AddCategory( AddCategoryDto categoryDto)
        {
            if (categoryDto == null)
                return BadRequest("Category shouldn't be empty.");

            var category = await _categoryServices.AddCategory(categoryDto, categoryDto.Image);

            return category == null
                ? BadRequest("Failed to save category.")
                : Ok(category);
        }








        [HttpPut("UpdateCategory/{id}")]
        public async Task<ActionResult<categoryDto>> UpdateCategory(int id, [FromForm] AddCategoryDto categoryDto)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            if (categoryDto == null)
                return BadRequest("Category shouldn't be empty.");

            var existingCategory = await _categoryServices.GetCategoryById(id);
            if (existingCategory == null)
                return NotFound("No category found.");

            var updatedCategory = await _categoryServices.UpdateCategory(id, categoryDto, categoryDto.Image);

            return updatedCategory == null
                ? BadRequest("Failed to update category.")
                : Ok(updatedCategory);
        }








        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<IReadOnlyList<categoryDto>>> GetCategoryById(int id)
        {
            try
            {


                if (id <= 0)
                {
                    return BadRequest("Invalid Id");
                }
                var category = await _categoryServices.GetCategoryById(id);

                if (category == null)
                {
                    return NotFound("No Category found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        [HttpGet("GetCategoryWithProductsById/{id}")]
        public async Task<ActionResult<CategoryWithProductsDto>> GetCategoryWithProductsById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid Id");

                var category = await _categoryServices.GetCategoryByIdWithInclude(id);

                if (category == null)
                    return NotFound("No Category found.");

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<categoryDto>>> GetAllCategories()
        {
            var categories = await _categoryServices.GetAllCategories();

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }

            return Ok(categories);
        }





        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");

            var existingCategory = await _categoryServices.GetCategoryById(id);
            if (existingCategory == null)
                return NotFound("No category found.");

            var result = await _categoryServices.DeleteCategory(id);

            return result == 0
                ? NotFound("Failed to delete category.")
                : Ok("Category deleted successfully.");
        }






    }
}
