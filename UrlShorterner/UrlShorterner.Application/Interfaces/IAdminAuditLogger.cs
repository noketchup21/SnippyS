using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface IAdminAuditLogger
    {
        Task LogAsync(Guid adminUserId, string action, string targetEntity, Guid targetId, object? details = null, CancellationToken ct = default);
    }
}
