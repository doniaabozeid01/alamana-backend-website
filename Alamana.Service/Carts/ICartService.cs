using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.CartItem.Dtos;
using Alamana.Service.Carts.Dtos;
using Alamana.Service.OperationResultService;

namespace Alamana.Service.Carts
{
    public interface ICartService
    {
        Task<GetCartDto> GetCartByUserId(string id);
        Task<GetCartDto> UpdateAmountOfCart(int id);
        Task<GetCartDto> AddCart(string userId);
        Task<GetCartDto> GetCartById(int id);
        Task<bool> DeleteCartByIdAsync(int id);
        Task<OperationResult<AddCartItemResultDto>> AddOrUpdateCartItemAsync(AddCartItem dto, CancellationToken ct = default);

    }
}
