using AutoMapper;
using E_Commerce.BLL.Models.ApplicationModels;
using E_Commerce.BLL.Models.ProfileModels;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Mapper
{
    public class DBMapper : Profile
    {
        public DBMapper()
        {
            CreateMap<ProductDTO, Product>().ReverseMap()
            .ForPath(dest => dest.categoryId, opts => opts.MapFrom(src => src.category.Id));
            CreateMap<CartDTO, Cart>().ReverseMap();
            CreateMap<CartItemDTO, CartItem>().ReverseMap();
            CreateMap<AddressDTO, Address>()
                .ReverseMap();
            CreateMap<InvoiceDTO, Invoice>().ReverseMap();
            CreateMap<PaymentCardDTO, PaymentCard>().ReverseMap();
            CreateMap<OrderDTO, Order>().ReverseMap();
            CreateMap<ReviewDTO, Review>().ReverseMap();
            CreateMap<StockDTO, Stock>().ReverseMap();
            CreateMap<StockItemDTO, StockItem>().ReverseMap();

            CreateMap<WishlistDTO, WishList>().ReverseMap();
            CreateMap<WishListItemDTO, WishListItem>().ReverseMap();

            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<ProfileDTO, ApplicationUser>()
                .ForPath(dest => dest.ProfilePhoto, opts => opts.MapFrom(src => src.PhotoPath))
                .ReverseMap();
            CreateMap<ChangePasswordDTO, ApplicationUser>().
                ForPath(user => user.PasswordChanged, opts => opts.MapFrom(dest => dest.ChangeTime))
               .ForPath(user => user.PassStrength, opts => opts.MapFrom(dest => dest.Strength)).ReverseMap();

        }
    }
}
