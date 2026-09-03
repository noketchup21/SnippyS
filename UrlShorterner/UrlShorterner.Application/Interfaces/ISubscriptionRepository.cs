using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task AddAsync(Subscription subscription, CancellationToken ct = default);
        Task<Subscription?> GetByProviderRefAsync(string providerRef, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
