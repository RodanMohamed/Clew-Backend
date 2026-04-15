using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clew.DAL
{
    public static class DALServicesExtension
    {
        public static void AddDALServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ClewConnection");

            // 1. Database + Seeding
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString)
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    if (await context.Set<Category>().AnyAsync())
                        return;

                    if (await context.Set<Product>().AnyAsync())
                        return;

                    var categories = SeedDataProvider.GetCategories();
                    var products = SeedDataProvider.GetProducts();

                    await context.AddRangeAsync(categories);
                    await context.SaveChangesAsync();

                    await context.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                })
                .UseSeeding((context, _) =>
                {
                    if (context.Set<Category>().Any())
                        return;

                    if (context.Set<Product>().Any())
                        return;

                    var categories = SeedDataProvider.GetCategories();
                    var products = SeedDataProvider.GetProducts();

                    context.AddRange(categories);
                    context.SaveChanges();

                    context.AddRange(products);
                    context.SaveChanges();
                });
            });

            // 2. Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // 3. UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}