using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using UrlShortener.Application.Interfaces;
using Stripe.Checkout;

namespace UrlShortener.Infrastructure.Services
{
    public class StripePaymentService
    {
        private readonly string _priceId;
        private readonly SessionService _sessionService;

        public StripePaymentService(IConfiguration configuration)
        {
            _priceId = configuration["Stripe:PlusPriceId"]
                ?? throw new InvalidOperationException("Stripe:PlusPriceId is not configured.");
            _sessionService = new SessionService();
        }

        // Kept fully intact for future use — not currently called anywhere while Stripe is disabled.
        public async Task<string> CreateCheckoutSessionAsync(Guid userId, string userEmail, CancellationToken ct = default)
        {
            var options = new SessionCreateOptions
            {
                Mode = "subscription",
                CustomerEmail = userEmail,
                LineItems = [new SessionLineItemOptions { Price = _priceId, Quantity = 1 }],
                SuccessUrl = "http://localhost:5173/account/upgrade/success",
                CancelUrl = "http://localhost:5173/account/upgrade/cancelled",
                ClientReferenceId = userId.ToString(),
                Metadata = new Dictionary<string, string> { { "userId", userId.ToString() } }
            };

            var session = await _sessionService.CreateAsync(options, cancellationToken: ct);
            return session.Url;
        }
    }
}
