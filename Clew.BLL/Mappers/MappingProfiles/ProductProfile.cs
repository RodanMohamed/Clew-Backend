using AutoMapper;
using Clew.DAL;

namespace Clew.BLL
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductEditDto, Product>();
            CreateMap<Product, ProductEditDto>();
        }
    }
}
