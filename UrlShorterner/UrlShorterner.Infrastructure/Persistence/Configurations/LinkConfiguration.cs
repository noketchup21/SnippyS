using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> e)
        {
            e.ToTable("links", t =>
                t.HasCheckConstraint("chk_short_code_format", "short_code ~ '^[A-Za-z0-9_-]+$'"));

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

            e.Property(x => x.ShortCode).HasMaxLength(16).IsRequired();
            e.HasIndex(x => x.ShortCode).IsUnique();

            e.Property(x => x.OriginalUrl).IsRequired();
            e.Property(x => x.VtStatus).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.AiSummary).HasMaxLength(500);
            e.Property(x => x.AiKeyTopics).HasMaxLength(300);
            e.Property(x => x.AiKeywords).HasMaxLength(300);
            e.Property(x => x.VtCheckAttempts).HasDefaultValue(0);

            e.HasOne(x => x.User)
                .WithMany(u => u.Links)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(x => x.UserId).HasFilter("user_id IS NOT NULL");
            e.HasIndex(x => x.VtStatus).HasFilter("vt_status IN ('Pending', 'Malicious', 'Unresolved')");
        }
    }
}
