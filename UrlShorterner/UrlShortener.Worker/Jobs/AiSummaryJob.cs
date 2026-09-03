using StackExchange.Redis;
using UrlShortener.Application.Interfaces;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Worker.Jobs;

public class AiSummaryJob : BackgroundService
{
    private const string ChannelName = "ai-summary-requests";
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AiSummaryJob> _logger;
    private readonly IConnectionMultiplexer _redis;

    public AiSummaryJob(IServiceProvider serviceProvider, ILogger<AiSummaryJob> logger, IConnectionMultiplexer redis)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _redis = redis;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        await subscriber.SubscribeAsync(RedisChannel.Literal(ChannelName), async (_, message) =>
        {
            if (!Guid.TryParse(message.ToString(), out var linkId)) return;
            await ProcessLinkAsync(linkId, stoppingToken);
        });

        _logger.LogInformation("AI summary job subscribed to {Channel}", ChannelName);

        // Also run a periodic sweep as a safety net — catches anything missed
        // if the Worker was down when a message was published (pub/sub has no persistence/replay)
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            await SweepMissedLinksAsync(stoppingToken);
        }
    }

    private async Task ProcessLinkAsync(Guid linkId, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var aiSummaryService = scope.ServiceProvider.GetRequiredService<IAiSummaryService>();

        var link = await db.Links.FindAsync([linkId], ct);
        if (link is null || link.AiSummaryAttempted) return;

        var summary = await aiSummaryService.GenerateSummaryAsync(link.OriginalUrl, ct);
        link.AiSummaryAttempted = true;

        if (summary is not null)
        {
            link.AiSummary = summary.Summary;
            link.AiKeyTopics = string.Join(", ", summary.KeyTopics);
            link.AiKeywords = string.Join(", ", summary.Keywords);
            link.AiEstimatedReadingMinutes = summary.EstimatedReadingMinutes;
            _logger.LogInformation("Generated AI summary for {ShortCode}", link.ShortCode);
        }
        else
        {
            _logger.LogWarning("Failed to generate AI summary for {ShortCode}", link.ShortCode);
        }

        await db.SaveChangesAsync();
    }

    private async Task SweepMissedLinksAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var linkRepository = scope.ServiceProvider.GetRequiredService<ILinkRepository>();
        var pending = await linkRepository.GetPendingAiSummaryLinksAsync(limit: 10, ct);

        foreach (var link in pending)
        {
            await ProcessLinkAsync(link.Id, ct);
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }
    }
}