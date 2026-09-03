using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _db;
        public AnalyticsRepository(AppDbContext db) => _db = db;

        public async Task<List<DailyStatDto>> GetDailyStatsAsync(Guid linkId, int days, CancellationToken ct = default)
        {
            var since = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
            return await _db.LinkClickDailyStats
                .Where(s => s.LinkId == linkId && s.StatDate >= since)
                .OrderBy(s => s.StatDate)
                .Select(s => new DailyStatDto(s.StatDate, s.ClickCount))
                .ToListAsync(ct);
        }
    }
}
