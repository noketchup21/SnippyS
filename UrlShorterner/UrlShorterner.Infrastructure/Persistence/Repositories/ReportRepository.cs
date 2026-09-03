using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _db;
        public ReportRepository(AppDbContext db) => _db = db;

        public Task<List<Report>> GetPendingAsync(CancellationToken ct = default) =>
            _db.Reports.Where(r => r.Status == Domain.Enums.ReportStatus.Pending).ToListAsync(ct);

        public Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Reports.Include(r => r.Link).FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task AddAsync(Report report, CancellationToken ct = default) =>
            await _db.Reports.AddAsync(report, ct);

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);
    }
}
