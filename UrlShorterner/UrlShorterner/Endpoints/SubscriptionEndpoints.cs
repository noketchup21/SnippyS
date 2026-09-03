using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;
using UrlShortener.Contracts.Subscriptions;

namespace UrlShortener.Endpoints
{
    public static class SubscriptionEndpoints
    {
        public static void MapSubscriptionEndpoints(this WebApplication app)
        {
            app.MapPost("/api/subscriptions/checkout", async (
                CreateCheckoutRequest request,
                [FromServices] IPaymentService paymentService,
                HttpContext http,
                CancellationToken ct) =>
            {
                if (!Enum.TryParse<PaymentProvider>(request.Provider, true, out var provider))
                    return Results.Problem(detail: "Invalid payment provider.", statusCode: 400);

                if (provider == PaymentProvider.Stripe)
                    return Results.Problem(detail: "Global payments are coming soon. Please use a Vietnamese payment method for now.", statusCode: 400);

                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
                var email = http.User.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value;
                var userId = Guid.Parse(sub);

                var checkoutUrl = await paymentService.CreateCheckoutSessionAsync(userId, email, provider, ct);
                return Results.Ok(new { checkoutUrl });
            }).RequireAuthorization();

            app.MapGet("/api/subscriptions/me", async (
                [FromServices] ISubscriptionRepository subscriptionRepository,
                HttpContext http,
                CancellationToken ct) =>
            {
                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
                var userId = Guid.Parse(sub);
                var tier = http.User.FindFirst("tier")?.Value ?? "Standard";

                var subscription = await subscriptionRepository.GetActiveByUserIdAsync(userId, ct);

                return Results.Ok(new SubscriptionStatusResponse(
                    tier,
                    subscription?.Status.ToString(),
                    subscription?.CurrentPeriodEnd));
            }).RequireAuthorization();
        }
    }
}
