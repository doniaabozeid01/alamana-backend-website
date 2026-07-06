using Alamana.Service.Category.Dtos;
using Alamana.Service.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alamana.Service.Carts;
using Alamana.Service.Carts.Dtos;
using Microsoft.AspNetCore.Identity;
using Alamana.Data.Entities;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        //private readonly ICategoryServices _categoryServices;

        public ICartService _cartService { get; }
        public UserManager<ApplicationUser> _userManager { get; }

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            //_categoryServices = categoryServices;
            _cartService = cartService;
            _userManager = userManager;
        }




        [HttpGet("GetOrCreateCart/{userId}")]
        public async Task<ActionResult<GetCartDto>> GetOrCreateCart(string userId, [FromQuery] int countryId)
        {
            if (countryId <= 0)
                return BadRequest(new { message = "countryId is required." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "User Not Found" });

            var cart = await _cartService.GetCartByUserId(userId, countryId);

            return cart == null
                ? NotFound(new { message = "Cart not found." })
                : Ok(cart);
        }

        [HttpGet("GetCartByUserId/{userId}")]
        public async Task<ActionResult<IReadOnlyList<GetCartDto>>> GetCartByUserId(string userId, [FromQuery] int countryId)
        {
            try
            {
                if (countryId <= 0)
                    return BadRequest(new { message = "countryId is required." });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) 
                    return NotFound(new { message = "User Not Found" });

                var cart = await _cartService.GetCartByUserId(userId, countryId);

                if (cart == null)
                    return NotFound(new { message = "No cart found." });

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
