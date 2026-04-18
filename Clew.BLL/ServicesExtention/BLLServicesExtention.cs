using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using AutoMapper;

namespace Clew.BLL
{
    public static class BLLServicesExtention
    {
       public static  void AddBLLServices(this IServiceCollection services)
    {
            services.AddScoped<ICategoryManager , CategoryManager>();
            services.AddScoped<ICartManager, CartManager>();
            services.AddScoped<IProductManager, ProductManager>();
            services.AddScoped<IImageManager , ImageManager>();
            services.AddScoped<IOrderManager, OrderManager>();
            services.AddValidatorsFromAssembly(typeof(BLLServicesExtention).Assembly);
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BLLServicesExtention).Assembly));
            
          
            services.AddScoped<IErrorMapper, ErrorMapper>();
        }
    }
}
