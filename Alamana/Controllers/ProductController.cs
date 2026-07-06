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

        private static bool IsValidCountryId(int countryId) => countryId > 0;

        [HttpPost("AddProduct")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> AddProduct([FromForm] AddProductDto productDto)
        {
            if (productDto == null)
                return BadRequest("product shouldn't be empty.");

            try
            {
                var product = await _productServices.AddProduct(productDto);

                return product == null
                    ? BadRequest("Failed to save product.")
                    : Ok(product);
            }
            
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("UpdateProduct/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromForm] UpdateProductDto productDto)
        {
            if (id <= 0) return BadRequest("Invalid ID");
            if (productDto == null) return BadRequest("product shouldn't be empty.");

            var existingProduct = await _productServices.GetProductById(id);
            if (existingProduct == null) return NotFound("No Product found.");

            try
            {
                var updated = await _productServices.UpdateProduct(id, productDto);
                return updated == null ? BadRequest("Failed to update product.") : Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProductById(int id, [FromQuery] int countryId)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid Id");

                if (!IsValidCountryId(countryId))
                    return BadRequest(new { message = "countryId is required." });

                var product = await _productServices.GetProductById(id, countryId);

                if (product == null)
                    return NotFound("No product found.");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts([FromQuery] int countryId, [FromQuery] int? categoryId = null)
        {
            if (!IsValidCountryId(countryId))
                return BadRequest(new { message = "countryId is required." });

            var products = await _productServices.GetAllProducts(categoryId, countryId);

            if (products == null || !products.Any())
                return NotFound("No products found.");

            return Ok(products);
        }

        [HttpGet("GetRandomProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetRandomProducts([FromQuery] int countryId)
        {
            if (!IsValidCountryId(countryId))
                return BadRequest(new { message = "countryId is required." });

            var products = await _productServices.GetRandomProducts(countryId);

            if (products == null || !products.Any())
                return NotFound("No products found.");

            return Ok(products);
        }

        [HttpGet("GetNewProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewProducts([FromQuery] int countryId)
        {
            if (!IsValidCountryId(countryId))
                return BadRequest(new { message = "countryId is required." });

            var products = await _productServices.GetNewProducts(countryId);
            return Ok(products);
        }

        [HttpGet("hero")]
        public async Task<ActionResult<ProductDto>> GetHeroProduct([FromQuery] int countryId)
        {
            if (!IsValidCountryId(countryId))
                return BadRequest(new { message = "countryId is required." });

            var product = await _productServices.GetHeroProductAsync(countryId);
            if (product == null)
                return NotFound(new { message = "No hero product found for this country." });

            return Ok(product);
        }

        [HttpDelete("DeleteProduct/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0) return BadRequest("Invalid Id");

            var ok = await _productServices.DeleteProduct(id);
            return ok ? NoContent() : NotFound("No product found or failed to delete.");
        }

        [HttpGet("best-sellers")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetBestSellers([FromQuery] int countryId, [FromQuery] int take = 5)
        {
            if (!IsValidCountryId(countryId))
                return BadRequest(new { message = "countryId is required." });

            var products = await _productServices.GetTopBestSellersAsync(take, countryId);
            return Ok(products);
        }
    }
}
