using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<List<Report>> GetPendingAsync(CancellationToken ct = default);
        Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Report report, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
