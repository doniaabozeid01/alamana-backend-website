using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.CartItem.Dtos;
using Alamana.Service.OperationResultService;

namespace Alamana.Service.CartItem
{
    public interface ICartItemsService
    {
        Task<GetCartItem> AddCartItemAsync(AddCartItem dto);
        Task<OperationResult<GetCartItem>> UpdateCartItemAsync(int id, int quantity);
        Task<GetCartItem> GetCartItemByIdAsync(int cartItemId);
        Task<int> RemoveCartItemAsync(int cartItemId);
        Task<int> ClearCartAsync(int cartId);

        Task<GetCartItem> GetCartItemByProductIdAsync(int productId);

    }
}
