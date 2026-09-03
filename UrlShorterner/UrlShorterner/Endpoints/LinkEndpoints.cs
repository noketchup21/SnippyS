using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Links;
using UrlShortener.Contracts.Links;

namespace UrlShortener.Endpoints
{
    public static class LinkEndpoints
    {
        public static void MapLinkEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/links").RequireRateLimiting("general");

            group.MapGet("/", async (
                [FromServices] ILinkRepository linkRepository,
                HttpContext http,
                [FromQuery] int page,
                [FromQuery] int pageSize,
                CancellationToken ct) =>
            {
                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(sub, out var userId))
                    return Results.Unauthorized();

                page = page < 1 ? 1 : page;
                pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

                var (items, totalCount) = await linkRepository.GetByUserIdPagedAsync(userId, page, pageSize, ct);

                return Results.Ok(new PagedLinksResponse(
                    items.Select(l => new LinkSummaryResponse(
                        l.Id, l.ShortCode, l.OriginalUrl, l.IsActive, l.IsCustomAlias,
                        l.PasswordHash is not null, l.VtStatus.ToString(), l.CreatedAt)).ToList(),
                    totalCount, page, pageSize));
            }).RequireAuthorization(); // no .RequireAuthorization() here — anonymous users must still be allowed through

            group.MapGet("/{id:guid}/qr", async (
                Guid id,
                [FromServices] ILinkRepository linkRepository,
                [FromServices] IQrCodeGenerator qrGenerator,
                HttpContext http,
                CancellationToken ct) =>
            {
                var isPlus = http.User.FindFirst("tier")?.Value == "Plus";
                if (!isPlus)
                    return Results.Problem(detail: "QR code generation requires a Plus subscription.", statusCode: 403);

                // Note: needs a GetByIdAsync method on ILinkRepository — add if not present
                var link = await linkRepository.GetByIdAsync(id, ct);
                if (link is null) return Results.NotFound();

                var redirectUrl = $"{http.Request.Scheme}://{http.Request.Host}/{link.ShortCode}";
                var png = qrGenerator.GeneratePng(redirectUrl);
                return Results.File(png, "image/png");
            }).RequireAuthorization();

            group.MapPost("/bulk", async (
                BulkCreateLinkRequest request,
                CreateShortLinkHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var isPlus = http.User.FindFirst("tier")?.Value == "Plus";
                if (!isPlus)
                    return Results.Problem(detail: "Bulk shorten requires a Plus subscription.", statusCode: 403);

                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userId = Guid.TryParse(sub, out var parsedId) ? parsedId : (Guid?)null;

                var results = new List<BulkCreateLinkResultItem>();

                foreach (var url in request.Urls)
                {
                    var command = new CreateShortLinkCommand(url, userId, null, IsPlus: true);
                    var result = await handler.HandleAsync(command, ct);

                    results.Add(result switch
                    {
                        CreateShortLinkResult.Success s => new BulkCreateLinkResultItem(url, true, s.ShortCode, null),
                        CreateShortLinkResult.InvalidUrl invalid => new BulkCreateLinkResultItem(url, false, null, invalid.Reason),
                        CreateShortLinkResult.MaliciousUrl malicious => new BulkCreateLinkResultItem(url, false, null, malicious.Reason),
                        CreateShortLinkResult.AliasTaken => new BulkCreateLinkResultItem(url, false, null, "Alias taken"),
                        _ => new BulkCreateLinkResultItem(url, false, null, "Unknown error")
                    });
                }

                return Results.Ok(results);
            }).RequireAuthorization();

            group.MapGet("/{id:guid}", async (
                Guid id,
                [FromServices] ILinkRepository linkRepository,
                HttpContext http,
                CancellationToken ct) =>
            {
                var link = await linkRepository.GetByIdAsync(id, ct);
                if (link is null) return Results.NotFound();

                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (link.UserId?.ToString() != sub) return Results.Forbid();

                return Results.Ok(new LinkDetailResponse(
                    link.Id, link.ShortCode, link.OriginalUrl, link.IsActive, link.IsCustomAlias,
                    link.PasswordHash is not null, link.VtStatus.ToString(), link.CreatedAt));
            }).RequireAuthorization();

            group.MapPost("/", async (
    CreateLinkRequest request,
    CreateShortLinkHandler handler,
    HttpContext http,
    CancellationToken ct) =>
            {
                Guid? userId = null;
                if (http.User.Identity?.IsAuthenticated == true)
                {
                    var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(sub, out var parsedId))
                        userId = parsedId;
                }

                var fingerprint = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var isPlus = http.User.FindFirst("tier")?.Value == "Plus";

                var command = new CreateShortLinkCommand(
                    request.OriginalUrl, userId, fingerprint, isPlus, request.CustomAlias, request.Password);

                var result = await handler.HandleAsync(command, ct);

                return result switch
                {
                    CreateShortLinkResult.Success s => Results.Created(
                        $"/api/links/{s.ShortCode}",
                        new LinkResponse(s.Id, s.ShortCode, s.OriginalUrl, s.CreatedAt)),
                    CreateShortLinkResult.QuotaExceeded => Results.Problem(
                        detail: "Link creation quota exceeded.", statusCode: StatusCodes.Status429TooManyRequests),
                    CreateShortLinkResult.InvalidUrl invalid => Results.Problem(
                        detail: invalid.Reason, statusCode: StatusCodes.Status400BadRequest),
                    CreateShortLinkResult.MaliciousUrl malicious => Results.Problem(
                        detail: malicious.Reason, statusCode: StatusCodes.Status422UnprocessableEntity),
                    CreateShortLinkResult.PlusFeatureRequired plus => Results.Problem(
                        detail: plus.Reason, statusCode: StatusCodes.Status403Forbidden),
                    CreateShortLinkResult.AliasTaken => Results.Problem(
                        detail: "This custom alias is already in use.", statusCode: StatusCodes.Status409Conflict),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
                };
            });

        }
    }
}
