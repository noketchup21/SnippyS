using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Worker.Jobs
{
    public class VirusTotalPollingJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VirusTotalPollingJob> _logger;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
        private const int MaxVtCheckAttempts = 3;

        public VirusTotalPollingJob(IServiceProvider serviceProvider, ILogger<VirusTotalPollingJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingLinksAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error during VirusTotal polling sweep");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingLinksAsync(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var linkRepository = scope.ServiceProvider.GetRequiredService<ILinkRepository>();
            var vtClient = scope.ServiceProvider.GetRequiredService<IVirusTotalClient>();

            var pendingLinks = await linkRepository.GetPendingVtLinksAsync(limit: 20, ct);
            if(pendingLinks.Count == 0)
            {
                _logger.LogInformation("No pending links for VirusTotal polling.");
                return;
            }

            _logger.LogInformation("Rechecking {Count} pending links against VirusTotal", pendingLinks.Count);

            foreach (var link in pendingLinks)
            {
                var result = await vtClient.CheckUrlAsync(link.OriginalUrl, ct);
                link.VtCheckAttempts++;

                switch (result.Outcome)
                {
                    case VtCheckOutcome.Clean:
                        link.VtStatus = VtStatus.Clean;
                        link.VtCheckedAt = DateTimeOffset.UtcNow;
                        break;

                    case VtCheckOutcome.Malicious:
                        link.VtStatus = VtStatus.Malicious;
                        link.VtCheckedAt = DateTimeOffset.UtcNow;
                        link.IsActive = false;
                        _logger.LogWarning("Link {ShortCode} flagged malicious, deactivated", link.ShortCode);
                        break;

                    case VtCheckOutcome.Pending:
                    case VtCheckOutcome.Error:
                        if (link.VtCheckAttempts >= MaxVtCheckAttempts)
                        {
                            link.VtStatus = VtStatus.Unresolved;
                            link.VtCheckedAt = DateTimeOffset.UtcNow;
                            _logger.LogWarning("Link {ShortCode} exhausted {Attempts} VT check attempts, marking Unresolved",
                                link.ShortCode, link.VtCheckAttempts);
                        }
                        break;
                }

                _logger.LogInformation("VT result for {ShortCode}: {Outcome} (attempt {Attempts}), new status will be {NewStatus}",
                    link.ShortCode, result.Outcome, link.VtCheckAttempts, link.VtStatus);

                await linkRepository.UpdateAsync(link, ct);

                await Task.Delay(TimeSpan.FromSeconds(15), ct);
            }

            await SaveChangesAsync(scope, ct);
        }

        private static async Task SaveChangesAsync(IServiceScope scope, CancellationToken ct)
        {
            var db = scope.ServiceProvider.GetRequiredService<UrlShortener.Infrastructure.Persistence.AppDbContext>();
            await db.SaveChangesAsync(ct);
        }
    }
}
