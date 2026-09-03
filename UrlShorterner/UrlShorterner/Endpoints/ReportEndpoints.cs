using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Reports;
using UrlShortener.Contracts.Reports;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Endpoints
{
    public static class ReportEndpoints
    {
        public static void MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/reports", async (
                        CreateReportRequest request,
                        [FromServices] CreateReportHandler handler,
                        [FromServices] ICaptchaVerifier captchaVerifier,
                        HttpContext http,
                        CancellationToken ct) =>
            {
                if (!await captchaVerifier.VerifyAsync(request.CaptchaToken, ct))
                    return Results.Problem(detail: "Captcha verification failed.", statusCode: 400);

                if (!Enum.TryParse<ReportReason>(request.Reason, ignoreCase: true, out var reason))
                    return Results.Problem(detail: "Invalid report reason.", statusCode: 400);

                Guid? reportedBy = null;
                if (http.User.Identity?.IsAuthenticated == true)
                {
                    var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(sub, out var parsedId))
                        reportedBy = parsedId;
                }

                var result = await handler.HandleAsync(
                    new CreateReportCommand(request.ShortCodeOrUrl, reportedBy, reason, request.Description), ct);

                return result switch
                {
                    CreateReportResult.Success s => Results.Created($"/api/reports/{s.Id}", new { id = s.Id }),
                    CreateReportResult.LinkNotFound => Results.Problem(
                        detail: "We couldn't find a link matching that URL or short code.", statusCode: 404),
                    _ => Results.Problem(statusCode: 500)
                };
            });
        }
    }
}
