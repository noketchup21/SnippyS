using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class RedisClickEventPublisher : IClickEventPublisher
    {
        private const string StreamKey = "clicks:stream";
        private readonly IConnectionMultiplexer _redis;

        public RedisClickEventPublisher(IConnectionMultiplexer redis) => _redis = redis;

        public async Task PublishAsync(ClickEvent clickEvent, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            await db.StreamAddAsync(StreamKey, new NameValueEntry[]
            {
                new ("linkId", clickEvent.LinkId.ToString()),
                new("clickedAt", clickEvent.ClickedAt.ToUnixTimeSeconds()),
                new("ipHash", clickEvent.IpHash ?? ""),
                new("country", clickEvent.Country ?? ""),
                new("deviceType", clickEvent.DeviceType ?? ""),
                new("browser", clickEvent.Browser ?? ""),
                new("referrer", clickEvent.Referrer ?? "")
            });
        }
    }
}
