using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using UrlShortener.Application.Subscriptions;

namespace UrlShortener.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/webhooks/stripe", async (
                HttpRequest request,
                [FromServices] HandleStripeWebhookHandler handler,
                [FromServices] IConfiguration configuration,
                CancellationToken ct) =>
            {
                var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
                var webhookSecret = configuration["Stripe:WebhookSecret"]!;

                Event stripeEvent;
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(
                        json,
                        request.Headers["Stripe-Signature"],
                        webhookSecret);
                }
                catch (StripeException)
                {
                    return Results.BadRequest("Invalid Stripe signature.");
                }

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        {
                            var session = stripeEvent.Data.Object as Session;
                            if (session?.Metadata.TryGetValue("userId", out var userIdStr) == true
                                && Guid.TryParse(userIdStr, out var userId)
                                && !string.IsNullOrEmpty(session.SubscriptionId))
                            {
                                var subscriptionService = new Stripe.SubscriptionService();
                                var stripeSubscription = await subscriptionService.GetAsync(session.SubscriptionId, cancellationToken: ct);

                                DateTimeOffset? periodEnd = stripeSubscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd is DateTime dt
                                    ? new DateTimeOffset(dt, TimeSpan.Zero)
                                    : null;

                                await handler.HandleCheckoutCompletedAsync(userId, session.SubscriptionId, periodEnd, ct);
                            }
                            break;
                        }
                    case "customer.subscription.deleted":
                        {
                            var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                            if (subscription is not null)
                                await handler.HandleSubscriptionCancelledAsync(subscription.Id, ct);
                            break;
                        }
                }

                return Results.Ok();
            });
            // Deliberately NO .RequireAuthorization() — Stripe can't send a JWT.
            // Security comes entirely from signature verification above.

            app.MapPost("/api/webhooks/payos", async (
                HttpRequest request,
                [FromServices] HandlePayOsWebhookHandler handler,
                [FromServices] IConfiguration configuration,
                CancellationToken ct) =>
            {
                var json = await new StreamReader(request.Body).ReadToEndAsync(ct);

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("data", out var data)) return Results.Ok();

                var orderCode = data.GetProperty("orderCode").GetInt64();
                var code = root.TryGetProperty("code", out var codeEl) ? codeEl.GetString() : null;

                if (code == "00")
                {
                    await handler.HandlePaymentSucceededAsync(orderCode, ct);
                }

                return Results.Ok();
            });
        }
    }
}
