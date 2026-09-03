using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class CompositePaymentService : IPaymentService
    {
        private readonly StripePaymentService _stripe;
        private readonly PayOsPaymentService _payOs;

        public CompositePaymentService(StripePaymentService stripe, PayOsPaymentService payOs)
        {
            _stripe = stripe;
            _payOs = payOs;
        }

        public async Task<string> CreateCheckoutSessionAsync(Guid userId, string userEmail, PaymentProvider provider, CancellationToken ct = default)
        {
            return provider switch
            {
                PaymentProvider.PayOS => await _payOs.CreatePaymentLinkAsync(userId, amountVnd: 99000, ct), // ~$3.99 equivalent, adjust as needed
                PaymentProvider.Stripe => throw new NotSupportedException("Stripe checkout is temporarily disabled. Coming soon."),
                _ => throw new ArgumentOutOfRangeException(nameof(provider))
            };
        }
    }
}
