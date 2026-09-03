using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations
{
    public class LinkClickConfiguration : IEntityTypeConfiguration<LinkClick>
    {
        public void Configure(EntityTypeBuilder<LinkClick> e)
        {
            e.ToTable("link_clicks");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).UseIdentityAlwaysColumn();
            e.Property(x => x.ClickedAt).HasDefaultValueSql("now()");

            e.HasIndex(x => new { x.LinkId, x.ClickedAt });
        }
    }
}
