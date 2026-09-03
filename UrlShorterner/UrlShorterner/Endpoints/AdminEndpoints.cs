using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Admin;
using UrlShortener.Application.Interfaces;
using UrlShortener.Contracts.Admin;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin").RequireAuthorization("AdminOnly");

            group.MapGet("/users", async ([FromServices] IUserRepository userRepository, CancellationToken ct) =>
            {
                var users = await userRepository.GetAllAsync(ct);
                return Results.Ok(users.Select(u => new { u.Id, u.Email, u.Role, u.Tier, u.IsBanned, u.CreatedAt }));
            });

            group.MapPatch("/users/{id:guid}/ban", async (
                Guid id,
                [FromServices] BanUserHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var adminId = Guid.Parse(http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                var result = await handler.HandleAsync(adminId, id, ban: true, ct);
                return result switch
                {
                    BanUserResult.Success => Results.NoContent(),
                    BanUserResult.UserNotFound => Results.NotFound(),
                    _ => Results.Problem(statusCode: 500)
                };
            });

            group.MapPatch("/users/{id:guid}/unban", async (
                Guid id,
                [FromServices] BanUserHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var adminId = Guid.Parse(http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                var result = await handler.HandleAsync(adminId, id, ban: false, ct);
                return result switch
                {
                    BanUserResult.Success => Results.NoContent(),
                    BanUserResult.UserNotFound => Results.NotFound(),
                    _ => Results.Problem(statusCode: 500)
                };
            });

            group.MapGet("/reports", async ([FromServices] IReportRepository reportRepository, CancellationToken ct) =>
            {
                var reports = await reportRepository.GetPendingAsync(ct);
                return Results.Ok(reports.Select(r => new
                {
                    r.Id,
                    r.LinkId,
                    r.ReportedByUserId,
                    Reason = r.Reason.ToString(),
                    r.Description,
                    Status = r.Status.ToString(),
                    r.CreatedAt
                }));
            });

            group.MapPost("/reports/{id:guid}/resolve", async (
                Guid id,
                [FromQuery] bool deactivateLink,
                [FromServices] ResolveReportHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var adminId = Guid.Parse(http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
                var result = await handler.HandleAsync(adminId, id, deactivateLink, ct);
                return result switch
                {
                    ResolveReportResult.Success => Results.NoContent(),
                    ResolveReportResult.ReportNotFound => Results.NotFound(),
                    _ => Results.Problem(statusCode: 500)
                };
            });

            group.MapGet("/logs", async ([FromServices] IAdminLogRepository logRepository, CancellationToken ct) =>
            {
                var logs = await logRepository.GetRecentAsync(count: 100, ct);
                return Results.Ok(logs);
            });

            group.MapGet("/links/unresolved", async (
                [FromServices] ILinkRepository linkRepository,
                CancellationToken ct) =>
            {
                var links = await linkRepository.GetByVtStatusAsync(VtStatus.Unresolved, ct);
                return Results.Ok(links.Select(l => new
                {
                    l.Id,
                    l.ShortCode,
                    l.OriginalUrl,
                    l.VtCheckAttempts,
                    l.VtCheckedAt,
                    l.CreatedAt,
                    l.IsActive
                }));
            });

            group.MapGet("/stats", async (
                [FromServices] IUserRepository userRepository,
                [FromServices] ILinkRepository linkRepository,
                [FromServices] IReportRepository reportRepository,
                CancellationToken ct) =>
            {
                var users = await userRepository.GetAllAsync(ct);
                var pendingReports = await reportRepository.GetPendingAsync(ct);

                var stats = new AdminStatsResponse(
                    TotalUsers: users.Count,
                    PlusUsers: users.Count(u => u.Tier == SubscriptionTier.Plus),
                    BannedUsers: users.Count(u => u.IsBanned),
                    TotalLinks: await linkRepository.CountAllAsync(ct),
                    ActiveLinks: await linkRepository.CountActiveAsync(true, ct),
                    InactiveLinks: await linkRepository.CountActiveAsync(false, ct),
                    CleanLinks: await linkRepository.CountByVtStatusAsync(VtStatus.Clean, ct),
                    PendingLinks: await linkRepository.CountByVtStatusAsync(VtStatus.Pending, ct),
                    MaliciousLinks: await linkRepository.CountByVtStatusAsync(VtStatus.Malicious, ct),
                    UnresolvedLinks: await linkRepository.CountByVtStatusAsync(VtStatus.Unresolved, ct),
                    PendingReports: pendingReports.Count);

                return Results.Ok(stats);
            });

            group.MapGet("/links/{id:guid}", async (
                Guid id,
                [FromServices] ILinkRepository linkRepository,
                CancellationToken ct) =>
            {
                var link = await linkRepository.GetByIdAsync(id, ct);
                if (link is null) return Results.NotFound();

                return Results.Ok(new
                {
                    link.Id,
                    link.ShortCode,
                    link.OriginalUrl,
                    link.UserId,
                    link.IsActive,
                    link.IsCustomAlias,
                    HasPassword = link.PasswordHash is not null,
                    VtStatus = link.VtStatus.ToString(),
                    link.VtCheckAttempts,
                    link.VtCheckedAt,
                    link.CreatedAt
                });
            });

            group.MapGet("/links/flagged", async (
                [FromServices] ILinkRepository linkRepository,
                CancellationToken ct) =>
            {
                var links = await linkRepository.GetByVtStatusesAsync(
                    [VtStatus.Malicious, VtStatus.Unresolved], ct);

                return Results.Ok(links.Select(l => new
                {
                    l.Id,
                    l.ShortCode,
                    l.OriginalUrl,
                    l.VtStatus,
                    l.VtCheckAttempts,
                    l.VtCheckedAt,
                    l.CreatedAt,
                    l.IsActive
                }));
            });

            group.MapGet("/reports/{id:guid}", async (
                Guid id,
                [FromServices] IReportRepository reportRepository,
                CancellationToken ct) =>
            {
                var report = await reportRepository.GetByIdAsync(id, ct);
                if (report is null) return Results.NotFound();

                return Results.Ok(new
                {
                    report.Id,
                    Reason = report.Reason.ToString(),
                    report.Description,
                    Status = report.Status.ToString(),
                    report.ReportedByUserId,
                    report.CreatedAt,
                    report.ResolvedAt,
                    Link = new
                    {
                        report.Link.Id,
                        report.Link.ShortCode,
                        report.Link.OriginalUrl,
                        report.Link.IsActive,
                        VtStatus = report.Link.VtStatus.ToString(),
                        report.Link.UserId
                    }
                });
            });
        }
    }
}
