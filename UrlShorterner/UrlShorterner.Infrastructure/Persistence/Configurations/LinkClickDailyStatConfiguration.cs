using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class LinkClickDailyStatConfiguration : IEntityTypeConfiguration<LinkClickDailyStat>
    {
        public void Configure(EntityTypeBuilder<LinkClickDailyStat> e)
        {
            e.ToTable("link_click_daily_stats");
            e.HasKey(x => new { x.LinkId, x.StatDate });

            e.Property(x => x.DeviceBreakdownJson)
                .HasColumnName("device_breakdown")
                .HasColumnType("jsonb");

            e.HasOne(x => x.Link)
                .WithMany()
                .HasForeignKey(x => x.LinkId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.LinkId, x.StatDate });
        }
    }
}
