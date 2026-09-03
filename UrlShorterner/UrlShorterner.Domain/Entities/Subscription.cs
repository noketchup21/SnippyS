using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionTier Tier { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
        public string? PaymentProviderRef { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? CurrentPeriodEnd { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }

        public User User { get; set; } = default!;
    }
}
