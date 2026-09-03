using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Worker.Jobs
{
    public class ClickAggregationJob : BackgroundService
    {
        private const string StreamKey = "clicks:stream";
        private const string ConsumerGroup = "aggregator-group";
        private const string ConsumerName = "aggregator-1";

        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ClickAggregationJob> _logger;

        public ClickAggregationJob(IServiceProvider serviceProvider, IConnectionMultiplexer redis, ILogger<ClickAggregationJob> logger)
        {
            _serviceProvider = serviceProvider;
            _redis = redis;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();
            await EnsureConsumerGroupExistsAsync(db);

            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var entries = await db.StreamReadGroupAsync(StreamKey, ConsumerGroup, ConsumerName, count: 100);

                    if(entries.Length > 0)
                    {
                        await ProcessBatchAsync(entries, stoppingToken);
                        foreach(var entry in entries)
                        {
                            await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, entry.Id);
                        }
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error processing click stream batch");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private static async Task EnsureConsumerGroupExistsAsync(IDatabase db)
        {
            try
            {
                await db.StreamCreateConsumerGroupAsync(StreamKey, ConsumerGroup, StreamPosition.NewMessages, createStream: true);
            }
            catch(RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
            {
                // group already exists — fine, ignore
            }
        }

        private async Task ProcessBatchAsync(StreamEntry[] entries, CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var clicks = new List<LinkClick>();

            foreach(var entry in entries)
            {
                var fields = entry.Values.ToDictionary(f => f.Name.ToString(), f => f.Value.ToString());

                if (!Guid.TryParse(fields.GetValueOrDefault("linkId"), out var linkId))
                    continue;

                var clickedAtUnix = long.TryParse(fields.GetValueOrDefault("clickedAt"), out var unixSeconds)
                    ? unixSeconds
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                clicks.Add(new LinkClick
                {
                    LinkId = linkId,
                    ClickedAt = DateTimeOffset.FromUnixTimeSeconds(clickedAtUnix),
                    IpHash = NullIfEmpty(fields.GetValueOrDefault("ipHash")),
                    Country = NullIfEmpty(fields.GetValueOrDefault("country")),
                    DeviceType = NullIfEmpty(fields.GetValueOrDefault("deviceType")),
                    Browser = NullIfEmpty(fields.GetValueOrDefault("browser")),
                    Referrer = NullIfEmpty(fields.GetValueOrDefault("referrer"))
                });
            }
            if (clicks.Count == 0) return;

            await dbContext.LinkClicks.AddRangeAsync(clicks, ct);
            await dbContext.SaveChangesAsync(ct);
            await UpsertDailyStatsAsync(dbContext, clicks, ct);

            _logger.LogInformation("Processed {Count} click events", clicks.Count);
        }

        private static async Task UpsertDailyStatsAsync(AppDbContext db, List<LinkClick> clicks, CancellationToken ct)
        {
            var groups = clicks.GroupBy(c => (c.LinkId, Date: DateOnly.FromDateTime(c.ClickedAt.UtcDateTime)));

            foreach(var group in groups)
            {
                var existing = await db.LinkClickDailyStats
                    .FirstOrDefaultAsync(s => s.LinkId == group.Key.LinkId && s.StatDate == group.Key.Date, ct);

                if(existing is null)
                {
                    db.LinkClickDailyStats.Add(new LinkClickDailyStat
                    {
                        LinkId = group.Key.LinkId,
                        StatDate = group.Key.Date,
                        ClickCount = group.Count()
                    });
                }
                else
                {
                    existing.ClickCount += group.Count();
                }
            }
            await db.SaveChangesAsync(ct);
        }
        private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;

    }
}
