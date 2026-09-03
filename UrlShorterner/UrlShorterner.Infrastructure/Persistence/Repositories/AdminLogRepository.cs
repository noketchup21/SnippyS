using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class AdminLogRepository : IAdminLogRepository
    {
        private readonly AppDbContext _db;
        public AdminLogRepository(AppDbContext db) => _db = db;

        public Task<List<AdminLog>> GetRecentAsync(int count, CancellationToken ct = default) =>
            _db.AdminLogs.OrderByDescending(l => l.CreatedAt).Take(count).ToListAsync(ct);
    }
}
