using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Subscriptions;

public record SubscriptionStatusResponse(
    string Tier,
    string? Status,
    DateTimeOffset? CurrentPeriodEnd);
