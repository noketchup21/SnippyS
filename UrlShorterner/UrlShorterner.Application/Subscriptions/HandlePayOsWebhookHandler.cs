using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Subscriptions
{
    public class HandlePayOsWebhookHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPayOsOrderRepository _orderRepository;

        public HandlePayOsWebhookHandler(
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            IPayOsOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _orderRepository = orderRepository;
        }

        public async Task<bool> HandlePaymentSucceededAsync(long orderCode, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByOrderCodeAsync(orderCode, ct);
            if (order is null || order.Status == "Paid") return false; // unknown order, or already processed (idempotency guard)

            var user = await _userRepository.GetByIdAsync(order.UserId, ct);
            if (user is null) return false;

            user.Tier = SubscriptionTier.Plus;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = order.UserId,
                Tier = SubscriptionTier.Plus,
                Status = SubscriptionStatus.Active,
                PaymentProviderRef = $"payos:{orderCode}",
                StartedAt = DateTimeOffset.UtcNow,
                CurrentPeriodEnd = DateTimeOffset.UtcNow.AddMonths(1) // PayOS one-time payment modeled as a 1-month grant; no recurring billing built in yet
            };

            order.Status = "Paid";
            order.CompletedAt = DateTimeOffset.UtcNow;

            await _subscriptionRepository.AddAsync(subscription, ct);
            await _userRepository.SaveChangesAsync(ct);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }
    }
}
