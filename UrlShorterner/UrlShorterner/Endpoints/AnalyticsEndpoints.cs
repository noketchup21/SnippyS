using UrlShortener.Application.Interfaces;

namespace UrlShortener.Endpoints
{
    public static class AnalyticsEndpoints
    {
        public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/links/{id:guid}/analytics", async (
                Guid id,
                IAnalyticsRepository repo,
                CancellationToken ct) =>
            {
                var stats = await repo.GetDailyStatsAsync(id, days: 30, ct);
                return Results.Ok(stats);
            }).RequireAuthorization();
        }
    }
}
