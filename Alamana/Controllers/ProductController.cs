using Alamana.Service.Category.Dtos;
using Alamana.Service.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alamana.Service.Product;
using Alamana.Service.Product.Dtos;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {



        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }




        [HttpPost("AddProduct")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> AddProduct([FromForm] AddProductDto productDto)
        {
            if (productDto == null)
                return BadRequest("product shouldn't be empty.");

            var product = await _productServices.AddProduct(productDto);

            return product == null
                ? BadRequest("Failed to save product.")
                : Ok(product);
        }









        [HttpPut("UpdateProduct/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromForm] UpdateProductDto productDto)
        {
            if (id <= 0) return BadRequest("Invalid ID");
            if (productDto == null) return BadRequest("product shouldn't be empty.");

            var existingProduct = await _productServices.GetProductById(id);
            if (existingProduct == null) return NotFound("No Product found.");

            var updated = await _productServices.UpdateProduct(id, productDto); // ✅ شيل البراميتر الثالث
            return updated == null ? BadRequest("Failed to update product.") : Ok(updated);
        }









        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProductById(int id)
        {
            try
            {


                if (id <= 0)
                {
                    return BadRequest("Invalid Id");
                }
                var product = await _productServices.GetProductById(id);

                if (product == null)
                {
                    return NotFound("No product found.");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


      



        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productServices.GetAllProducts();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }





        [HttpGet("GetRandomProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetRandomProducts()
        {
            var products = await _productServices.GetRandomProducts();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }




        [HttpDelete("DeleteProduct/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id");

            var ok = await _productServices.DeleteProduct(id);
            return ok ? NoContent() : NotFound("No product found or failed to delete.");
        }






    }
}
