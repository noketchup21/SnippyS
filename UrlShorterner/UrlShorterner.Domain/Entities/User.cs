using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.User;
        public SubscriptionTier Tier { get; set; } = SubscriptionTier.Standard;
        public bool IsBanned { get; set; }
        public bool EmailVerified { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public ICollection<Link> Links { get; set; } = [];
        public ICollection<Subscription> Subscriptions { get; set; } = [];
    }
}
