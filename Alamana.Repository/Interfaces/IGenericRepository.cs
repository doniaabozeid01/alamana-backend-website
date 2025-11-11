using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;

namespace Alamana.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        void Update(TEntity entity);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        void Delete(TEntity entity);





        Task<CartItems> GetCartByUserIdAndProuctIdAsync(int cartId, int productId);
        Task<Cart> GetCartByUserId(string userId);
        Task<Products> GetProductByIdAsync(int productId);

        Task<Categories> GetCategoryByIdAsync(int id);


        //Task<Cart> GetCartByUserIdAsync(string id);

        Task ClearByCartIdAsync(int cartId);
        Task<Cart> GetCartByIdAsync(int id);



        Task<CartItems> GetCartItemByProductIdAsync(int productId);






        Task<IReadOnlyList<Governorate>> GetGovernoratesByCountryId(int CountryId);
        Task<IReadOnlyList<city>> GetCitiesByGovernorateId(int GovernorateId);
        Task<IReadOnlyList<District>> GetDistrictsByCityId(int CityId);



        Task<IReadOnlyList<Products>> GetAllProductsAsync();

        //IQueryable<BaseEntity> Query();

        IQueryable<TEntity> Query(); // 👈 دي ترجع IQueryable من الجدول

        Task<IReadOnlyList<Products>> GetRandomProductsAsync();
        Task<IReadOnlyList<Products>> GetNewProducts();


        Task<FavouriteProducts> GetProductFavourite(int productId, string userId);

        Task<IReadOnlyList<FavouriteProducts>> GetProductFavouriteByUserId(string userId);

        Task<FavouriteProducts> GetProductFavouriteByProductIdAndUserId(int productId, string userId);


    }
}
