using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence
{
    public class AdminLogConfiguration : IEntityTypeConfiguration<AdminLog>
    {
        public void Configure(EntityTypeBuilder<AdminLog> e)
        {
            e.ToTable("admin_logs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).UseIdentityAlwaysColumn();

            e.Property(x => x.DetailsJson)
                .HasColumnName("details")
                .HasColumnType("jsonb");

            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

            e.HasIndex(x => x.AdminUserId);
            e.HasIndex(x => new { x.TargetEntity, x.TargetId });
            e.HasIndex(x => x.CreatedAt).IsDescending();
        }
    }
}
