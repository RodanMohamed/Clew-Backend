using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Clew.DAL;
using Clew.Common;
using System.Text.Json;
namespace Clew.BLL
{
     public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderReadDto>();

            CreateMap<OrderItem, OrderItemReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.ProductName)
                        ? src.ProductName
                        : (src.Product != null ? src.Product.Name : string.Empty)))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.ProductImage)
                        ? src.ProductImage
                        : (src.Product != null ? src.Product.Image : null)));

            CreateMap<ShippingAddressDto, Address>().ReverseMap();

           
            CreateMap<PlaceOrderDto, Order>()
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress));
        }
    }
}
