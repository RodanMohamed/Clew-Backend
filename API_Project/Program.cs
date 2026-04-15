using Clew.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace Clew.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Controllers
            builder.Services.AddControllers();

            // 2. Scalar / OpenAPI
            builder.Services.AddOpenApi();

            // 3. DAL Services (DbContext + Repositories + UnitOfWork + Seeding)
            builder.Services.AddDALServices(builder.Configuration);

            // 4. Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

          
         

            var app = builder.Build();

            // 8. Seed database on startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.EnsureCreatedAsync();
            }

            // 9. Scalar UI (dev only)
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "Clew API";
                    options.Theme = ScalarTheme.Purple;
                    options.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.Fetch);
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("ClewPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}