using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Repository.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AlamanaBbContext _context;

        public GenericRepository(AlamanaBbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }


        public async Task<CartItems> GetCartByUserIdAndProuctIdAsync(int cartId, int productId)
        {
            return await _context.Set<CartItems>().FirstOrDefaultAsync(x => x.cartId == cartId && x.productId == productId);
        }



        public async Task<Cart> GetCartByUserId(string userId)
        {
            return await _context.Set<Cart>().FirstOrDefaultAsync(x => x.userId == userId);
        }



        public async Task<Products> GetProductByIdAsync(int productId)
        {
            return await _context.Set<Products>().Include(x => x.Category).Include(x=>x.Media).FirstOrDefaultAsync(x => x.Id == productId);
        }



        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }


        public async Task<IReadOnlyList<TEntity>> GetAllAsync ()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }


        public async Task<IReadOnlyList<Products>> GetAllProductsAsync()
        {
            return await _context.Set<Products>().Include(x=>x.Category).Include(x => x.Media).ToListAsync();
        }


        public async Task<IReadOnlyList<Products>> GetRandomProductsAsync()
        {
            return await _context.Set<Products>().OrderBy(p => Guid.NewGuid()).Take(5).Include(x => x.Media).Include(x => x.Category).ToListAsync();
        }



        public async Task<IReadOnlyList<Products>> GetNewProducts()
        {
            return await _context.Set<Products>().Include(x=>x.Media).Include(x => x.Category).Where(x => x.New == true).ToListAsync();
        }


        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }


        public async Task<Categories> GetCategoryByIdAsync(int id)
        {
            return await _context.Set<Categories>().Include(x => x.Products).ThenInclude(c=>c.Media.Where(x=>x.Type == Data.Enums.MediaType.Image)).FirstOrDefaultAsync(x => x.Id == id);
        }



        //public async Task<Cart> GetCartByUserIdAsync(string id)
        //{
        //    return await _context.Set<Cart>().Include(x => x.cartItems).FirstOrDefaultAsync(x => x.userId == id);
        //}


        public Task ClearByCartIdAsync(int cartId)
        {
            var items = _context.CartItems.Where(c => c.cartId == cartId).ToList();
            _context.CartItems.RemoveRange(items);
            return Task.CompletedTask; // شيلنا حفظ التغييرات
        }


        public async Task<Cart> GetCartByIdAsync(int id)
        {
            return await _context.Set<Cart>().Include(x => x.cartItems).FirstOrDefaultAsync(x => x.Id == id);
        }



        public async Task<CartItems> GetCartItemByProductIdAsync(int productId)
        {
            return await _context.Set<CartItems>().FirstOrDefaultAsync(x => x.productId == productId);
        }


        public async Task<IReadOnlyList<District>> GetDistrictsByCityId(int CityId)
        {
            return await _context.Set<District>().Where(x => x.CityId == CityId).ToListAsync();
        }


        public async Task<IReadOnlyList<city>> GetCitiesByGovernorateId(int GovernorateId)
        {
            return await _context.Set<city>().Where(x => x.GovernorateId == GovernorateId).ToListAsync();
        }



        public async Task<IReadOnlyList<Governorate>> GetGovernoratesByCountryId(int CountryId)
        {
            return await _context.Set<Governorate>().Where(x => x.CountryId == CountryId).ToListAsync();
        }



        public IQueryable<TEntity> Query()
        {
            return _context.Set<TEntity>().AsQueryable();
        }







        public async Task<FavouriteProducts> GetProductFavouriteByProductIdAndUserId(int productId, string userId)
        {
            return await _context.Set<FavouriteProducts>().Include(x=>x.Product).Include(x=>x.User).FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);
        }





        public async Task<IReadOnlyList<FavouriteProducts>> GetProductFavouriteByUserId(string userId)
        {
            return await _context.Set<FavouriteProducts>()
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                // ⬅️ هنا الفلترة على التجميعة BranchStocks
                .Include(f => f.Product).ThenInclude(p => p.Media)
                // (اختياري) لو EF Core 5+: Include مُفلتر لصف مخزون الفرع فقط
                .ToListAsync();
        }




        public async Task<FavouriteProducts> GetProductFavourite(int productId, string userId)
        {
            return await _context.Set<FavouriteProducts>().Where(x => x.UserId == userId && x.ProductId == productId ).FirstOrDefaultAsync();
        }




    }

}
