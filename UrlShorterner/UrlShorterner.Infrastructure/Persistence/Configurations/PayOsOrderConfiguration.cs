using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class PayOsOrderConfiguration : IEntityTypeConfiguration<PayOsOrder>
    {
        public void Configure(EntityTypeBuilder<PayOsOrder> e)
        {
            e.ToTable("payos_orders");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

            e.HasIndex(x => x.OrderCode).IsUnique();

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
