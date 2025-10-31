using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Alamana.Repository.Interfaces;
using Alamana.Service.CartItem.Dtos;
using Alamana.Service.OperationResultService;
using AutoMapper;
using static System.Net.Mime.MediaTypeNames;

namespace Alamana.Service.CartItem
{
    public class CartItemsService : ICartItemsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }












        public async Task<GetCartItem> AddCartItemAsync(AddCartItem dto)
        {
            var product = await _unitOfWork.Repository<Products>().GetByIdAsync(dto.productId);
            if (product == null) { }

            var cart = await _unitOfWork.Repository<Cart>().GetCartByUserId(dto.userId);
            if (cart == null) { }


            var newCart = new Cart
            {
                userId = dto.userId,
                TotalAmount = 0,
                CreateAt = DateTime.Now,
            };


            await _unitOfWork.Repository<Cart>().AddAsync(newCart);



            //var cartItems = await _unitOfWork.Repository<CartItems>().getcart(dto. ,dto.productId);
            var newItem = new CartItems
            {
                //cartId = dto.cartId, 
                productId = dto.productId,
                Quantity = dto.Quantity,
                Price = product.Price,
                ImagePath = product.ImagePathCover,
                Name = product.Name,
                TotalPrice = dto.Quantity * product.Price,
            };
            await _unitOfWork.Repository<CartItems>().AddAsync(newItem);
            await _unitOfWork.CompleteAsync();

            var mappedItem = _mapper.Map<GetCartItem>(newItem);
            return mappedItem;

        }





        public async Task<GetCartItem> UpdateCartItemAsync(int id ,int quantity)
        {
            var existingItem = await _unitOfWork.Repository<CartItems>().GetByIdAsync(id);

            // Get current price/discount from stock
            //var stock = await _unitOfWork.Repository<items_stock>().GetByIdAsync(dto.ItemsStockId);
            //if (stock == null) throw new Exception("Invalid stock item");

            existingItem.Quantity = quantity;
            existingItem.TotalPrice = quantity * existingItem.Price;
            _unitOfWork.Repository<CartItems>().Update(existingItem);
            await _unitOfWork.CompleteAsync();


            var mappedItem = _mapper.Map<GetCartItem>(existingItem);
            return mappedItem;

        }







        public async Task<GetCartItem> GetCartItemByIdAsync(int cartItemId)
        {
            var items = await _unitOfWork.Repository<CartItems>().GetByIdAsync(cartItemId);

            // Map manually or use AutoMapper if configured

            //var cartItemsDto = new GetCartItem
            //{
            //    Id = cartId,
            //}

            var mappedItem = _mapper.Map<GetCartItem>(items);
            return mappedItem;
        }




        public async Task<GetCartItem> GetCartItemByProductIdAsync(int productId)
        {
            var item = await _unitOfWork.Repository<CartItems>().GetCartItemByProductIdAsync(productId);
            var mappedItem = _mapper.Map<GetCartItem>(item);
            return mappedItem;
        }





        public async Task<int> RemoveCartItemAsync(int cartItemId)
        {
            var item = await _unitOfWork.Repository<CartItems>().GetByIdAsync(cartItemId);
            if (item != null)
            {
                _unitOfWork.Repository<CartItems>().Delete(item);
                var result = await _unitOfWork.CompleteAsync();
                return result;
            }
            return 0;
        }

        public async Task<int> ClearCartAsync(int cartId)
        {
            await _unitOfWork.Repository<CartItems>().ClearByCartIdAsync(cartId);
            var result = await _unitOfWork.CompleteAsync();
            return result;
        }

        Task<OperationResult<GetCartItem>> ICartItemsService.UpdateCartItemAsync(int id, int quantity)
        {
            throw new NotImplementedException();
        }
    }


}
