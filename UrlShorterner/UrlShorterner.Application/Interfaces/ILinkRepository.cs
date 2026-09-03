using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Interfaces
{
    public interface ILinkRepository
    {
        Task<Link?> GetByShortCodeAsync(string shortCode, CancellationToken ct = default);
        Task AddAsync(Link link, CancellationToken ct = default);
        Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<Link?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Link>> GetPendingVtLinksAsync(int limit, CancellationToken ct = default);
        Task UpdateAsync(Link link, CancellationToken ct = default);
        Task<List<Link>> GetByVtStatusAsync(VtStatus status, CancellationToken ct = default);
        Task<List<Link>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<int> CountByVtStatusAsync(VtStatus status, CancellationToken ct = default);
        Task<int> CountActiveAsync(bool isActive, CancellationToken ct = default);
        Task<int> CountAllAsync(CancellationToken ct = default);
        Task<List<Link>> GetByVtStatusesAsync(IEnumerable<VtStatus> statuses, CancellationToken ct = default);
        Task<(List<Link> Items, int TotalCount)> GetByUserIdPagedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
        Task<List<Link>> GetPendingAiSummaryLinksAsync(int limit, CancellationToken ct = default);
        Task<Link?> GetByShortCodeWithOwnerAsync(string shortCode, CancellationToken ct = default);
    }
}
