using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _db;
        public SubscriptionRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Subscription subscription, CancellationToken ct = default) =>
            await _db.Subscriptions.AddAsync(subscription, ct);

        public Task<Subscription?> GetByProviderRefAsync(string providerRef, CancellationToken ct = default) =>
            _db.Subscriptions.FirstOrDefaultAsync(s => s.PaymentProviderRef == providerRef, ct);

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);

        public Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            _db.Subscriptions
        .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
        .OrderByDescending(s => s.StartedAt)
        .FirstOrDefaultAsync(ct);
    }
}
