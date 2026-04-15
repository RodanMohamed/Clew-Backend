using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Tax).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Total).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Status).HasDefaultValue("pending");

            // Owned entity — stored as columns in Orders table ////////////////////
            builder.OwnsOne(o => o.ShippingAddress);

            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
