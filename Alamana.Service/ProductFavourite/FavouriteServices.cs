using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.Product.Dtos;
using Alamana.Service.ProductFavourite.Dtos;
using AutoMapper;

namespace Alamana.Service.ProductFavourite
{
    public class FavouriteServices : IFavouriteServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AlamanaBbContext _context;

        public FavouriteServices(IUnitOfWork unitOfWork, IMapper mapper, AlamanaBbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }


        public async Task<FavouriteDto> AddFavouriteProduct(AddFavouriteDto favouriteDto)
        {

            var favourite = _mapper.Map<FavouriteProducts>(favouriteDto);

            await _unitOfWork.Repository<FavouriteProducts>().AddAsync(favourite);
            var status = await _unitOfWork.CompleteAsync();

            if (status == 0)
            {
                return null;
            }

            return new FavouriteDto
            {
                Id = favourite.Id,
                ProductId = favourite.ProductId,
                UserId = favourite.UserId,
                Product = _mapper.Map<ProductDto>(favourite.Product),
                User = _mapper.Map<UserDto>(favourite.User),
                AddedOn = favourite.AddedOn,
            };
        }





        public async Task<IReadOnlyList<FavouriteDto>> GetProductFavouriteByUserId(string userId)
        {
            // 1) هات المفضّلة للمستخدم (الريبو بتاعك أصلاً بيرجّع Product + Images + BranchStocks مُفلتره بالفرع)
            var favEntities = await _unitOfWork.Repository<FavouriteProducts>().GetProductFavouriteByUserId(userId);

            if (favEntities == null || favEntities.Count == 0)
                return Array.Empty<FavouriteDto>();

            // 2) العملة للفرع (مرة واحدة)

            // 3) تكوين الـDTO يدويًا
            var result = new List<FavouriteDto>(favEntities.Count);

            foreach (var fav in favEntities)
            {
                var p = fav.Product;
                // بما إن Include مفلتر، الـ BranchStocks هنا (لو وُجدت) غالبًا عنصر واحد لنفس الفرع
                //var bs = p?.BranchStocks?.FirstOrDefault(bs => bs.BranchId == branchId);

                decimal old = p?.Price ?? 0m;

                var firstImage = p?.Media?
                                   .OrderBy(i => i.Id)
                                   .Select(i => i.Url)
                                   .FirstOrDefault();

                // ‼️ FavouriteDto عندك حقوله أرقام int، لو عايزاها أدق خلّيها decimal
                var dto = new FavouriteDto
                {
                    Id = fav.Id,
                    ProductId = fav.ProductId,
                    name = p?.Name,
                    oldPrice = (int)Math.Round(old),
                    imagePath = firstImage,
                    UserId = fav.UserId,
                    // لو محتاجة الكائنات كاملة غيريهم لـ DTOs بدل الكيانات
                    User = null,          // or map to UserDto لو عملتي Include(f => f.User)
                    AddedOn = fav.AddedOn,
                };

                result.Add(dto);
            }

            return result;
        }
































        public async Task<FavouriteDto> GetProductFavouriteById(int id)
        {
            var favourite = await _unitOfWork.Repository<FavouriteProducts>().GetByIdAsync(id);
            var mappedFavourite = _mapper.Map<FavouriteDto>(favourite);
            return mappedFavourite;
        }




        public async Task<FavouriteDto> GetProductFavourite(int productId, string userId)
        {
            var favourite = await _unitOfWork.Repository<FavouriteProducts>().GetProductFavourite(productId, userId);
            var mappedFavourite = _mapper.Map<FavouriteDto>(favourite);
            return mappedFavourite;
        }


        public async Task<FavouriteDto> GetProductFavouriteByProductIdAndUserId(int productId, string userId)
        {
            var favourite = await _unitOfWork.Repository<FavouriteProducts>().GetProductFavouriteByProductIdAndUserId(productId, userId);
            var mappedFavourite = _mapper.Map<FavouriteDto>(favourite);
            return mappedFavourite;
        }


        public async Task<int> DeleteProductFavourite(int id)
        {
            var fav = await _unitOfWork.Repository<FavouriteProducts>().GetByIdAsync(id);

            if (fav == null)
            {
                return 0;
            }

            _unitOfWork.Repository<FavouriteProducts>().Delete(fav);
            return await _unitOfWork.CompleteAsync();
        }
    }

}
