using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Subscriptions
{
    public record CreateCheckoutRequest(string Provider);
}
