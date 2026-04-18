using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class AppDbContext :  IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public AppDbContext() : base()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public override int SaveChanges()
        {
            AuditLog();
            return base.SaveChanges();
        }
        private void AuditLog()
        {
            var dateTime = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = dateTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = dateTime;
                }

            }
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        }
    }
}
