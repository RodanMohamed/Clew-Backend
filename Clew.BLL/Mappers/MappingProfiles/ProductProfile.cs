using AutoMapper;
using Clew.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductReadDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
