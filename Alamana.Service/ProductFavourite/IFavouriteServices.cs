using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Service.ProductFavourite.Dtos;

namespace Alamana.Service.ProductFavourite
{
    public interface IFavouriteServices
    {
        Task<FavouriteDto> AddFavouriteProduct(AddFavouriteDto favouriteDto);
        Task<IReadOnlyList<FavouriteDto>> GetProductFavouriteByUserId(string id);
        Task<FavouriteDto> GetProductFavouriteById(int id);
        Task<FavouriteDto> GetProductFavourite(int productId, string userId);
        Task<FavouriteDto> GetProductFavouriteByProductIdAndUserId(int productId, string userId);
        Task<int> DeleteProductFavourite(int id);

    }
}
