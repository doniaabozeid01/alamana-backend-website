using Alamana.Service.CartItem.Dtos;
using Alamana.Service.CartItem;
using Alamana.Service.Carts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alamana.Service.Product;
using Alamana.Data.Entities;
using CloudinaryDotNet;
using Alamana.Repository.Repositories;
using Alamana.Repository.Interfaces;
using Alamana.Service.OperationResultService;

namespace Alamana.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly ICartService _cartService;
        private readonly IProductServices _productServices;
        private readonly IUnitOfWork _unitOfWork;

        public CartItemsController(ICartItemsService cartItemsService, ICartService cartService, IProductServices productServices, IUnitOfWork unitOfWork)
        {
            _cartItemsService = cartItemsService;
            _cartService = cartService;
            _productServices = productServices;
            _unitOfWork = unitOfWork;

        }

        // GET: api/cartitem/{cartId}
        //[HttpGet("[controller]/GetCartItemById/{cartId}")]
        //public async Task<IActionResult> GetCartItemById(int cartId)
        //{

        //    var items = await _cartItemsService.GetCartItemByIdAsync(cartId);
        //    if (items == null)
        //    {
        //        return NotFound(new { message = "cart items not found" });
        //    }
        //    return Ok(items);
        //}









        [HttpPost("[controller]/AddCartItem")]
        public async Task<IActionResult> AddCartItem([FromBody] AddCartItem dto)
        {
            if (dto == null || dto.Quantity <= 0)
                return BadRequest(new { success = false, message = "كمية غير صحيحة" });

            try
            {
                var result = await _cartService.AddOrUpdateCartItemAsync(dto);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new
                {
                    success = true,
                    message = "item added successfully",
                    messageAr = "تم إضافة/تحديث المنتج في السلة بنجاح",
                    item = result.Data.Item,            // الـ CartItem بعد التحديث
                    cart = result.Data.CartSummary      // ملخص السلة (الإجمالي، عدد العناصر…)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ غير متوقع", detail = ex.Message });
            }
        }







        //[HttpPost("[controller]/UpdateCartItem/{id}")]
        //public async Task<IActionResult> UpdateCartItem(int id, int quantity)
        //{
        //    var cartItem = await _cartItemsService.GetCartItemByIdAsync(id);
        //    if (cartItem == null)
        //        return NotFound(new { message = "Cart item not found" });

        //    if (quantity < 1)
        //        return BadRequest(new { message = "Quantity must be greater than 0" });

        //    var result = await _cartItemsService.UpdateCartItemAsync(id, quantity);

        //    if (!result.Success)
        //        return BadRequest(new { success = false, message = result.Message });

        //    await _cartService.UpdateAmountOfCart(cartItem.cartId);
        //    await _unitOfWork.CompleteAsync();

        //    return Ok(new { success = true, message = "Product Quantity updated successfully", item = result.Data });
        //}








        // DELETE: api/cartitem/{id}
        [HttpDelete("[controller]/DeleteCartItem/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _cartItemsService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound(new { message = "cart Item not found" });
            }
            var result = await _cartItemsService.RemoveCartItemAsync(id);
            if (result == 0)
            {
                return BadRequest(new { message = "item deleted failed" });

            }

            await _cartService.UpdateAmountOfCart(cartItem.cartId);


            var cart = await _cartService.GetCartById(cartItem.cartId);
            if (cart != null && cart.TotalAmount == 0)
            {
                await _cartService.DeleteCartByIdAsync(cartItem.cartId);

            }

            await _unitOfWork.CompleteAsync();



            return Ok(new { message = "item deleted successfully", messageAr = "تم حذف المنتج بنجاح" });
        }







        // DELETE: api/cartitem/clear/{cartId}
        [HttpDelete("[controller]/ClearCart/{cartId}")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var cart = await _cartService.GetCartById(cartId);
            if (cart == null)
            {
                return NotFound(new { message = "cart not found" });
            }

            var result = await _cartItemsService.ClearCartAsync(cartId);
            if (result < 1)
            {
                return BadRequest(new { message = "an error occured in clearing the cart item" });
            }


            await _cartService.UpdateAmountOfCart(cartId);
            await _unitOfWork.CompleteAsync();

            return Ok(new { messageEn = "Cart deleted successfully" });
        }






    }
}
