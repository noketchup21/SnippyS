using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public Guid? ReportedByUserId { get; set; }
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public Guid? ResolvedByAdminId { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Link Link { get; set; } = default!;
    }
}
