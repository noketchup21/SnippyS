using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Entities
{
    public class AdminLog
    {
        public long Id { get; set; }
        public Guid AdminUserId { get; set; }
        public string Action { get; set; } = default!;
        public string TargetEntity { get; set; } = default!;
        public Guid TargetId { get; set; }
        public string? DetailsJson { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
