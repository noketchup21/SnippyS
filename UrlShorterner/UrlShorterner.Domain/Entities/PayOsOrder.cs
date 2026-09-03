using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Domain.Entities
{
    public class PayOsOrder
    {
        public Guid Id { get; set; }
        public long OrderCode { get; set; }
        public Guid UserId { get; set; }
        public long AmountVnd { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }

        public User User { get; set; } = default!;
    }
}
