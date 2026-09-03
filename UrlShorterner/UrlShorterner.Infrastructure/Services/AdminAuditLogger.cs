using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Services
{
    public class AdminAuditLogger : IAdminAuditLogger
    {
        private readonly AppDbContext _db;
        public AdminAuditLogger(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(Guid adminUserId, string action, string targetEntity, Guid targetId, object? details = null, CancellationToken ct = default)
        {
            _db.AdminLogs.Add(new AdminLog
            {
                AdminUserId = adminUserId,
                Action = action,
                TargetEntity = targetEntity,
                TargetId = targetId,
                DetailsJson = details is null ? null : JsonSerializer.Serialize(details),
                CreatedAt = DateTimeOffset.UtcNow
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}
