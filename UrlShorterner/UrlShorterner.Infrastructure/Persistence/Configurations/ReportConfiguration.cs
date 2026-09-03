using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> e)
        {
            e.ToTable("reports");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

            e.Property(x => x.Reason).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

            e.HasOne(x => x.Link)
                .WithMany(l => l.Reports)
                .HasForeignKey(x => x.LinkId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.Status).HasFilter("status = 'Pending'");
            e.HasIndex(x => x.LinkId);
        }
    }
}
