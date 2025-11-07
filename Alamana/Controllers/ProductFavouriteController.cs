using Alamana.Service.ProductFavourite.Dtos;
using Alamana.Service.ProductFavourite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFavouriteController : ControllerBase
    {

            private readonly IFavouriteServices _favouriteServices;

            public ProductFavouriteController(IFavouriteServices favouriteServices)
            {
                _favouriteServices = favouriteServices;
            }

 



            [HttpGet("GetProductFavouriteByUserId/{userId}")]
            public async Task<ActionResult<IReadOnlyList<FavouriteDto>>> GetProductFavouriteByUserId(string userId)
            {
                var favProduct = await _favouriteServices.GetProductFavouriteByUserId(userId);
                if (favProduct == null)
                {
                    return NotFound($"favourite proucts with ID {userId} not found.");
                }
                return Ok(favProduct);
            }





            [HttpPost("AddProductFavourite")]
            public async Task<ActionResult> AddProductFavourite(AddFavouriteDto favouriteDto)
            {
                if (favouriteDto == null)
                {
                    return BadRequest("favourite Product is required.");
                }
                var favProduct = await _favouriteServices.GetProductFavouriteByProductIdAndUserId(favouriteDto.ProductId, favouriteDto.UserId);
                if (favProduct != null)
                {
                    return Ok(new { message = "this product already in your favourite list" });
                }
                var addedFav = await _favouriteServices.AddFavouriteProduct(favouriteDto);
                if (addedFav == null)
                {
                    return StatusCode(500, "Failed to add favourite.");
                }

                var favoriteProduct = new FavouriteDto
                {
                    Id = addedFav.Id,
                    ProductId = favouriteDto.ProductId,
                    UserId = favouriteDto.UserId,

                };

                return Ok(new
                {
                    success = true,
                    messageEn = "Product added to wishlist successfully.",
                    messageAr = "تم إضافة المنتج إلى قائمة المفضلة بنجاح.",
                    data = favoriteProduct
                });

            }





            [HttpDelete("DeleteProductFromFavourite")]
            public async Task<ActionResult<IReadOnlyList<FavouriteDto>>> DeleteProductFromFavourite([FromQuery] int productId, [FromQuery] string userId)
            {
                var fav = await _favouriteServices.GetProductFavourite(productId, userId);
                if (fav == null)
                {
                    return NotFound(new { message = $"Favourite not found." });
                }
                //var userId = fav.UserId;

                var result = await _favouriteServices.DeleteProductFavourite(fav.Id);
                if (result == 0)
                {
                    return StatusCode(500, new { message = "Failed to delete favourite product." });
                }

                var favouritesOfUser = await _favouriteServices.GetProductFavouriteByUserId(userId);
                if (favouritesOfUser == null)
                {
                    return NotFound(new { message = $"there are no Advertises" });

                }
                return Ok(new
                {
                    messageEn = "Product deleted from wishlist successfully",
                    messageAr = "تم حذف المنتج من قائمة المفضلة بنجاح.",
                    data = favouritesOfUser
                });
                // حذف الصورة بعد التأكد من حذف الإعلان
                //_advertiseServices.DeleteImage(advert.ImagePath);

            }








            [HttpGet("GetProductFavouriteByUserIdAndProductId/{userId}")]
            public async Task<ActionResult<IReadOnlyList<FavouriteDto>>> GetProductFavouriteByUserIdAndProductId(string userId, int productId)
            {
                var favProduct = await _favouriteServices.GetProductFavouriteByProductIdAndUserId(productId, userId);
                if (favProduct == null)
                {
                    return NotFound(new { message = $"favourite proucts with ID {productId} not found." });
                }
                return Ok(favProduct);
            }



    }

}

