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
            // Entity to DTO
            //CreateMap<Order, OrderReadDto>()
            //    .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => 
            //        JsonSerializer.Deserialize<ShippingAddressDto>(src.ShippingAddress ?? "{}")));
            
            CreateMap<OrderItem, OrderItemReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown"))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product != null ? src.Product.Image : null));

            // DTO to Entity
            CreateMap<PlaceOrderDto, Order>()
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => 
                    JsonSerializer.Serialize(src.ShippingAddress)));
        }
    }
}
