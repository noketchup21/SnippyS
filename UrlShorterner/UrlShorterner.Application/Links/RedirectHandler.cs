using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Links;

public abstract record LinkRedirectOutcome
{
    public record Found(
        Guid LinkId,
        string OriginalUrl,
        string? AiSummary,
        List<string>? AiKeyTopics,
        int? AiEstimatedReadingMinutes,
        bool OwnerIsPlus,
        bool AiSummaryAttempted) : LinkRedirectOutcome;
    public record NotFound : LinkRedirectOutcome;
    public record Inactive : LinkRedirectOutcome;
    public record PasswordRequired : LinkRedirectOutcome;
    public record IncorrectPassword : LinkRedirectOutcome;
}

public class RedirectHandler
{
    private readonly ILinkRepository _linkRepository;
    private readonly IClickEventPublisher _clickEventPublisher;
    private readonly IPasswordHasher _passwordHasher;

    public RedirectHandler(
        ILinkRepository linkRepository,
        IClickEventPublisher clickEventPublisher,
        IPasswordHasher passwordHasher)
    {
        _linkRepository = linkRepository;
        _clickEventPublisher = clickEventPublisher;
        _passwordHasher = passwordHasher;
    }

    // Step 1: resolve the link, check password — does NOT log a click or redirect yet
    public async Task<LinkRedirectOutcome> ResolveAsync(string shortCode, string? password, CancellationToken ct = default)
    {
        var link = await _linkRepository.GetByShortCodeWithOwnerAsync(shortCode, ct);
        if (link is null) return new LinkRedirectOutcome.NotFound();
        if (!link.IsActive) return new LinkRedirectOutcome.Inactive();

        if (link.PasswordHash is not null)
        {
            if (password is null) return new LinkRedirectOutcome.PasswordRequired();
            if (!_passwordHasher.Verify(password, link.PasswordHash)) return new LinkRedirectOutcome.IncorrectPassword();
        }

        var topics = string.IsNullOrEmpty(link.AiKeyTopics)
            ? null
            : link.AiKeyTopics.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();

        var ownerIsPlus = link.User?.Tier == SubscriptionTier.Plus;

        return new LinkRedirectOutcome.Found(
            link.Id, link.OriginalUrl, link.AiSummary, topics,
            link.AiEstimatedReadingMinutes, ownerIsPlus, link.AiSummaryAttempted);
    }

    // Step 2: called only after the interstitial's captcha is confirmed — logs the click
    public async Task LogClickAsync(Guid linkId, RecordClickCommand command, CancellationToken ct = default)
    {
        var clickEvent = new ClickEvent(
            LinkId: linkId,
            ClickedAt: DateTimeOffset.UtcNow,
            IpHash: command.IpAddress is null ? null : HashIp(command.IpAddress),
            Country: null,
            DeviceType: ParseDeviceType(command.UserAgent),
            Browser: null,
            Referrer: command.Referrer);

        await _clickEventPublisher.PublishAsync(clickEvent, ct);
    }

    private static string HashIp(string ip)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(ip));
        return Convert.ToHexString(bytes);
    }

    private static string? ParseDeviceType(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return null;
        return userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ? "mobile" : "desktop";
    }
}