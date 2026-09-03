using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public enum PaymentProvider { Stripe, PayOS }

    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(Guid userId, string userEmail, PaymentProvider provider, CancellationToken ct = default);
    }
}
