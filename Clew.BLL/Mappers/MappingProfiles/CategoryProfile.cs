using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Clew.DAL;
using Clew.BLL;

namespace Clew.BLL
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryReadDto>();

            CreateMap<Category, CategoryEditDto>();

            CreateMap<CategoryCreateDto, Category>();

            CreateMap<CategoryEditDto, Category>();
        }
    }
}
