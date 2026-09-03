using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities
{
    public class Link
    {
        public Guid Id { get; set; }
        public string ShortCode { get; set; } = default!;
        public string OriginalUrl { get; set; } = default!;
        public Guid? UserId { get; set; }
        public bool IsCustomAlias { get; set; }
        public string? PasswordHash { get; set; }
        public VtStatus VtStatus { get; set; } = VtStatus.Unchecked;
        public DateTimeOffset? VtCheckedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int VtCheckAttempts { get; set; }

        public string? AiSummary { get; set; }
        public string? AiKeyTopics { get; set; } // comma-separated or JSON array as string
        public string? AiKeywords { get; set; }
        public int? AiEstimatedReadingMinutes { get; set; }
        public bool AiSummaryAttempted { get; set; } //don't retry endlessly on failure

        public User? User { get; set; }
        public ICollection<Report> Reports { get; set; } = [];
    }
}
