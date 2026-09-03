using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class RedisQuotaService : IQuotaService
    {
        private const int AnonymousLimit = 2;
        private const int StandardDailyLimit = 10;
        private readonly IConnectionMultiplexer _redis;

        public RedisQuotaService(IConnectionMultiplexer redis) => _redis = redis;

        public async Task<bool> TryConsumeAnonymousQuotaAsync(string fingerprint, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var key = $"anon:{fingerprint}:count";

            var count = await db.StringIncrementAsync(key);
            if (count ==1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromDays(30)); // window for "before signup"
            }
            return count <= AnonymousLimit;
        }

        public async Task<bool> TryConsumeStandardQuotaAsync(Guid userId, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var key = $"quota:{userId}:{today}";

            var count = await db.StringIncrementAsync(key);
            if (count == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromHours(25)); // safety margin past midnight
            }
            return count <= StandardDailyLimit;
        }
    }
}
