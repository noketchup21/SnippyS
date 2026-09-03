using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Links;
using UrlShortener.RedirectService.Rendering;

namespace UrlShortener.RedirectService.Endpoints;

public static class RedirectEndpoints
{
    public static void MapRedirectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{shortCode}", async (
            string shortCode,
            [FromQuery] string? password,
            [FromServices] RedirectHandler handler,
            [FromServices] IConfiguration configuration,
            CancellationToken ct) =>
        {
            var turnstileSiteKey = configuration["Turnstile:SiteKey"]
                ?? throw new InvalidOperationException("Turnstile:SiteKey is not configured.");

            var result = await handler.ResolveAsync(shortCode, password, ct);

            return result switch
            {
                LinkRedirectOutcome.Found f => Results.Content(
                    RedirectPageRenderer.RenderInterstitialPage(
                        shortCode, f.LinkId, f.OriginalUrl, turnstileSiteKey,
                        f.AiSummary, f.AiKeyTopics, f.AiEstimatedReadingMinutes,
                        f.OwnerIsPlus, f.AiSummaryAttempted),
                    "text/html"),
                LinkRedirectOutcome.NotFound => Results.NotFound(),
                LinkRedirectOutcome.Inactive => Results.Content(
                    RedirectPageRenderer.RenderMessagePage("This link is no longer active."),
                    "text/html", statusCode: 410),
                LinkRedirectOutcome.PasswordRequired => Results.Content(
                    RedirectPageRenderer.RenderPasswordPromptPage(shortCode, wrongPassword: false), "text/html"),
                LinkRedirectOutcome.IncorrectPassword => Results.Content(
                    RedirectPageRenderer.RenderPasswordPromptPage(shortCode, wrongPassword: true), "text/html"),
                _ => Results.Problem(statusCode: 500)
            };
        }).RequireRateLimiting("redirects");

        app.MapPost("/{shortCode}/continue", async (
            string shortCode,
            [FromForm] string linkId,
            [FromForm] string destination,
            [FromForm(Name = "cf-turnstile-response")] string? captchaToken,
            [FromServices] RedirectHandler handler,
            [FromServices] ICaptchaVerifier captchaVerifier,
            [FromServices] IConfiguration configuration,
            HttpContext http,
            CancellationToken ct) =>
        {
            if (string.IsNullOrEmpty(captchaToken) || !await captchaVerifier.VerifyAsync(captchaToken, ct))
            {
                var turnstileSiteKey = configuration["Turnstile:SiteKey"]!;
                return Results.Content(
                    RedirectPageRenderer.RenderInterstitialPage(
                        shortCode, Guid.Parse(linkId), destination, turnstileSiteKey,
                        null, null, null, ownerIsPlus: false, aiSummaryAttempted: false, captchaError: true),
                    "text/html", statusCode: 400);
            }

            var command = new RecordClickCommand(
                shortCode,
                http.Connection.RemoteIpAddress?.ToString(),
                http.Request.Headers.UserAgent.ToString(),
                http.Request.Headers.Referer.ToString(),
                null);

            await handler.LogClickAsync(Guid.Parse(linkId), command, ct);

            return Results.Redirect(destination, permanent: false);
        })
        .RequireRateLimiting("redirects")
        .DisableAntiforgery();
    }
}