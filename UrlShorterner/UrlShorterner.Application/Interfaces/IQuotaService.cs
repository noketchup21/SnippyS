using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface IQuotaService
    {
        // Returns true if the caller is allowed to create a link, false if quota exceeded.
        Task<bool> TryConsumeAnonymousQuotaAsync(string fingerprint, CancellationToken ct = default);
        Task<bool> TryConsumeStandardQuotaAsync(Guid userId, CancellationToken ct = default);
    }
}
