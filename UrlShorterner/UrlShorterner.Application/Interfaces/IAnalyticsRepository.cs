using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public record DailyStatDto(DateOnly Date, int ClickCount);

    public interface IAnalyticsRepository
    {
        Task<List<DailyStatDto>> GetDailyStatsAsync(Guid linkId, int days, CancellationToken ct = default);
    }
}
