using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities
{
    public class VerificationCode
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = default!;
        public VerificationCodeType Type { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; } = default!;
    }
}
