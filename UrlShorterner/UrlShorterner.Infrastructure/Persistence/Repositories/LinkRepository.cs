using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly AppDbContext _db;
        public LinkRepository(AppDbContext db) => _db = db;
        private const int MaxVtCheckAttempts = 3;

        public Task<Link?> GetByShortCodeAsync(string shortCode, CancellationToken ct = default) =>
            _db.Links.FirstOrDefaultAsync(l => l.ShortCode == shortCode, ct);

        public async Task AddAsync(Link link, CancellationToken ct = default) =>
            await _db.Links.AddAsync(link, ct);

        public Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken ct = default) =>
            _db.Links.AnyAsync(l => l.ShortCode == shortCode, ct);

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);

        public Task<Link?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Links.FirstOrDefaultAsync(l => l.Id == id, ct);

        public Task<List<Link>> GetPendingVtLinksAsync(int limit, CancellationToken ct = default) =>
            _db.Links
                .Where(l => l.VtStatus == VtStatus.Pending && l.VtCheckAttempts < MaxVtCheckAttempts)
                .OrderBy(l => l.CreatedAt)
                .Take(limit)
                .ToListAsync(ct); // use EntityFrameworkQueryableExtensions.ToListAsync explicitly if the CS0411 ambiguity resurfaces

        public Task UpdateAsync(Link link, CancellationToken ct = default)
        {
            _db.Links.Update(link);
            return Task.CompletedTask;
        }

        public Task<List<Link>> GetByVtStatusAsync(VtStatus status, CancellationToken ct = default) =>
            _db.Links
        .Where(l => l.VtStatus == status)
        .OrderByDescending(l => l.CreatedAt)
        .ToListAsync(ct);

        public Task<List<Link>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            _db.Links
        .Where(l => l.UserId == userId)
        .OrderByDescending(l => l.CreatedAt)
        .ToListAsync(ct);

        public Task<int> CountByVtStatusAsync(VtStatus status, CancellationToken ct = default) =>
            _db.Links.CountAsync(l => l.VtStatus == status, ct);

        public Task<int> CountActiveAsync(bool isActive, CancellationToken ct = default) =>
            _db.Links.CountAsync(l => l.IsActive == isActive, ct);

        public Task<int> CountAllAsync(CancellationToken ct = default) =>
            _db.Links.CountAsync(ct);

        public Task<List<Link>> GetByVtStatusesAsync(IEnumerable<VtStatus> statuses, CancellationToken ct = default) =>
             _db.Links
        .Where(l => statuses.Contains(l.VtStatus))
        .OrderByDescending(l => l.CreatedAt)
        .ToListAsync(ct);

        public async Task<(List<Link> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _db.Links.Where(l => l.UserId == userId).OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return (items, totalCount);
        }
        public async Task<List<Link>> GetPendingAiSummaryLinksAsync(int limit, CancellationToken ct = default) =>
            await _db.Links
            .Where(l => !l.AiSummaryAttempted && l.User != null && l.User.Tier == SubscriptionTier.Plus)
            .OrderBy(l => l.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

        public Task<Link?> GetByShortCodeWithOwnerAsync(string shortCode, CancellationToken ct = default) =>
            _db.Links.Include(l => l.User).FirstOrDefaultAsync(l => l.ShortCode == shortCode, ct);
    }
}
