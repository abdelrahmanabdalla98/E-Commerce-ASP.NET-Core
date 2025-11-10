using AutoMapper;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Repository;
using E_Commerce.BLL.RepositoryPattern;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace E_Commerce.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly INormalRepository<CartDTO, Cart> _Repo;
        private readonly IMapper Mapper;
        private CartDTO _CartDTO;
        public CartService(INormalRepository<CartDTO,Cart> Repo, IMapper Mapper)
        {
            this._Repo = Repo;
            this.Mapper = Mapper;
            _CartDTO = new CartDTO();
        }

        public async Task<bool> CreateCart()
        {
            return await _Repo.CreateAsync(_CartDTO);
        }

        public void DeleteFromCart(CartItemDTO t)
        {
            var cartItem = Mapper.Map<CartItem>(t);
            _CartDTO.Total -= cartItem.Subtotal;
            _CartDTO.Items.Remove(cartItem);
        }
        public async Task<CartDTO> ViewCart(Expression<Func<Cart, bool>> filter, bool notrack = false, Func<IQueryable<Cart>, IQueryable<Cart>>? includeQuery = null)
        {
            return await _Repo.GetAsync(filter, notrack, includeQuery);
        }

        public async Task<bool> UpdateCart(CartDTO t)
        {
            return await _Repo.UpdateAsync(t);
        }

        public void AddToCart(CartItemDTO t ,string UserID)
        {
           var cartItem = Mapper.Map<CartItem>(t);
           _CartDTO.Items.Add(cartItem);
            foreach(var item in _CartDTO.Items)
            {
                _CartDTO.Subtotal += cartItem.Subtotal;
            }
            _CartDTO.Total = _CartDTO.Subtotal + _CartDTO.ShippingTax + _CartDTO.VAT;
           _CartDTO.ApplicationUserId = UserID;        
        }
    }
    public interface ICartService
    {
        public Task<CartDTO> ViewCart(Expression<Func<Cart, bool>> filter
            , bool notrack = false
            , Func<IQueryable<Cart>, IQueryable<Cart>>? includeQuery = null);
        public Task<bool> UpdateCart(CartDTO t);
        public void AddToCart(CartItemDTO t, string UserID);
        public Task<bool> CreateCart();

        public void DeleteFromCart(CartItemDTO t);
    }
}
