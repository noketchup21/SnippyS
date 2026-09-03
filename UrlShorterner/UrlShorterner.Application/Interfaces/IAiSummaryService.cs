using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public record AiSummaryResult(
        string Summary,
        List<string> KeyTopics,
        List<string> Keywords,
        int EstimatedReadingMinutes);

    public interface IAiSummaryService
    {
        Task<AiSummaryResult?> GenerateSummaryAsync(string url, CancellationToken ct = default);
    }
}
