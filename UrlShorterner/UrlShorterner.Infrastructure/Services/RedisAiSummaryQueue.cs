using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class RedisAiSummaryQueue : IAiSummaryQueue
    {
        private readonly string ChannelName = "ai-summary-requests";
        private readonly IConnectionMultiplexer _reids;

        public RedisAiSummaryQueue(IConnectionMultiplexer reids)
        {
            _reids = reids;
        }

        public async Task EnqueueAsync(Guid linkId, CancellationToken ct = default)
        {
            var subscriber = _reids.GetSubscriber();
            await subscriber.PublishAsync(RedisChannel.Literal(ChannelName), linkId.ToString());
        }
    }
}
