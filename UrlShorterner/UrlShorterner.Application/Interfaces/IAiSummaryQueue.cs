using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface IAiSummaryQueue
    {
        Task EnqueueAsync(Guid linkId, CancellationToken ct = default);
    }
}
