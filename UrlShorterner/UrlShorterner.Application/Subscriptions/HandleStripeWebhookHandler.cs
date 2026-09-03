using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Subscriptions
{
    public class HandleStripeWebhookHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public HandleStripeWebhookHandler(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task HandleCheckoutCompletedAsync(Guid userId, string stripeSubscriptionId, DateTimeOffset? periodEnd, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                return; //log later
            }

            user.Tier = SubscriptionTier.Plus;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Tier = SubscriptionTier.Plus,
                Status = SubscriptionStatus.Active,
                PaymentProviderRef = stripeSubscriptionId,
                StartedAt = DateTimeOffset.UtcNow,
                CurrentPeriodEnd = periodEnd
            };

            await _subscriptionRepository.AddAsync(subscription, ct);
            await _userRepository.SaveChangesAsync(ct); // both tracked by same DbContext, one SaveChanges commits both
        }

        public async Task HandleSubscriptionCancelledAsync(string stripeSubscriptionId, CancellationToken ct = default)
        {
            var subscription = await _subscriptionRepository.GetByProviderRefAsync(stripeSubscriptionId, ct);
            if (subscription is null) return;

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTimeOffset.UtcNow;

            var user = await _userRepository.GetByIdAsync(subscription.UserId, ct);
            if(user is not null)
            {
                user.Tier = SubscriptionTier.Standard;
                user.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _subscriptionRepository.SaveChangesAsync(ct);
        }
    }
}
