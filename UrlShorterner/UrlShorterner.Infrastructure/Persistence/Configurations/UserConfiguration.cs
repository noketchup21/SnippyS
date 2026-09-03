using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> e)
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

            e.Property(x => x.Email)
                .HasColumnType("citext")
                .IsRequired();
            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Tier).HasConversion<string>().HasMaxLength(20);

            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");

            e.HasIndex(x => x.Role).HasFilter("role = 'Admin'");
        }
    }
}
