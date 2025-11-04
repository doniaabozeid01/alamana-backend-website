using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Context;
using Alamana.Data.Entities;
using Alamana.Data.Enums;
using Alamana.Repository.Interfaces;
using Alamana.Service.CartItem.Dtos;
using Alamana.Service.Carts.Dtos;
using Alamana.Service.Category.Dtos;
using Alamana.Service.OperationResultService;
using Alamana.Service.Product.Dtos;
using Alamana.Service.SaveAndDeleteImage;
using AutoMapper;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Service.Carts
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AlamanaBbContext _context;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, AlamanaBbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }




        public async Task<GetCartDto> AddCart(string userId)
        {

            var cart = new Cart
            {
                userId = userId,
                TotalAmount = 0,
                CreateAt = DateTime.Now,
            };

            await _unitOfWork.Repository<Cart>().AddAsync(cart);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<GetCartDto>(cart);
        }






        public async Task<GetCartDto> UpdateAmountOfCart(int id)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(id);
            if (cart == null)
                return null;



            // Load CartItems
            var cartItems = await _unitOfWork.Repository<CartItems>().GetCartByIdAsync(id); // ← تأكد إن دي بتجيب cart items فعليًا

            // Calculate total
            var total = cartItems.cartItems.Sum(item => item.TotalPrice);


            // Update cart
            cart.TotalAmount = Math.Round(total, 2);
            _unitOfWork.Repository<Cart>().Update(cart);
            var status = await _unitOfWork.CompleteAsync();

            return status == 0 ? null : _mapper.Map<GetCartDto>(cart);
        }






        public async Task<GetCartDto> GetCartByUserId(string id)
        {
            //var cart = await _unitOfWork.Repository<Cart>().GetCartByUserIdAsync(id);

            var cart = await _context.Cart
                .Where(c => c.userId == id)
                .Select(c => new GetCartDto
                {
                    Id = c.Id,
                    TotalAmount = c.TotalAmount,
                    CreateAt = c.CreateAt,
                    cartItems = c.cartItems.Select(i => new GetCartItem
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        TotalPrice = i.TotalPrice,
                        ImagePath = i.ImagePath,
                        productId = i.productId
                    }).ToList()
                })
                .FirstOrDefaultAsync();


            if (cart == null)
                return null;

            var mappedCart = _mapper.Map<GetCartDto>(cart);
            return mappedCart;
        }



        public async Task<GetCartDto> GetCartById(int id)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(id);

            if (cart == null)
                return null;

            var mappedCart = _mapper.Map<GetCartDto>(cart);
            return mappedCart;
        }





        public async Task<bool> DeleteCartByIdAsync(int id)
        {
            // لو عندك Table/Query:
            var cartRepo = _unitOfWork.Repository<Cart>();
            //var itemRepo = _unitOfWork.Repository<CartItems>();

            var cart = await cartRepo
                .Query()                                   // أو .Query()
                .Include(c => c.cartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return false;

            //// لو مش مفعّل Cascade Delete على FK بتاع CartItems → احذف العناصر يدويًا
            //if (cart.cartItems != null && cart.cartItems.Count > 0)
            //{
            //    // لو عندك DeleteRange:
            //    // itemRepo.DeleteRange(cart.cartItems);

            //    foreach (var ci in cart.cartItems)
            //        itemRepo.Delete(ci);
            //}

            cartRepo.Delete(cart);

            await _unitOfWork.CompleteAsync();
            return true;
        }





        public async Task<OperationResult<AddCartItemResultDto>> AddOrUpdateCartItemAsync(AddCartItem dto, CancellationToken ct = default)
        {
            // 1) التحقق من المنتج
            var productRepo = _unitOfWork.Repository<Products>();
            var product = await productRepo.GetByIdAsync(dto.productId);
            if (product == null)
                return OperationResult<AddCartItemResultDto>.Fail("المنتج غير موجود");

            // 2) هات/أنشئ السلة
            var cartRepo = _unitOfWork.Repository<Cart>();
            var cart = await cartRepo
                .Query() 
                .Include(c => c.cartItems)
                .FirstOrDefaultAsync(c => c.userId == dto.userId, ct);

            if (cart == null)
            {
                cart = new Cart
                {
                    userId = dto.userId,
                    TotalAmount = 0m,
                    CreateAt = DateTime.UtcNow
                };
                await cartRepo.AddAsync(cart);
                await _unitOfWork.CompleteAsync();
            }

            var itemRepo = _unitOfWork.Repository<CartItems>();
            var existingItem = cart.cartItems?.FirstOrDefault(i => i.productId == dto.productId);

            if (existingItem == null)
            {
                var newItem = new CartItems
                {
                    cartId = cart.Id,                         
                    productId = dto.productId,
                    Quantity = dto.Quantity,
                    Price = product.Price,                   
                    ImagePath = product.Media.FirstOrDefault(x=>x.Type == MediaType.Image)?.Url,
                    Name = product.Name,
                    TotalPrice = product.Price * dto.Quantity
                };

                await itemRepo.AddAsync(newItem);
            }
            else
            {
                existingItem.Quantity += dto.Quantity;       
                existingItem.Price = product.Price;          
                existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
                itemRepo.Update(existingItem);
            }

            await _unitOfWork.CompleteAsync();

            // 4) حدّث إجمالي السلة
            // الأفضل تجمع من الداتا مباشرة لضمان الدقة
            var total = await itemRepo
                .Query( )
                .Where(i => i.cartId == cart.Id)
                .SumAsync(i => (decimal?)i.TotalPrice, ct) ?? 0m;

            cart.TotalAmount = total;
            cartRepo.Update(cart);
            await _unitOfWork.CompleteAsync();

            // 5) رجّع النتيجة مابّنج DTO
            // رجّع آخر حالة للعنصر المعني
            var savedItem = await itemRepo
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.cartId == cart.Id && i.productId == dto.productId, ct);

            var mappedItem = _mapper.Map<GetCartItem>(savedItem);

            var cartSummary = new CartSummaryDto
            {
                CartId = cart.Id,
                TotalAmount = cart.TotalAmount,
                ItemsCount = await itemRepo.Query().CountAsync(i => i.cartId == cart.Id, ct)
            };



            return OperationResult<AddCartItemResultDto>.SuccessResult(new AddCartItemResultDto
            {
                Item = mappedItem,
                CartSummary = cartSummary
            });



        }




    }


}
