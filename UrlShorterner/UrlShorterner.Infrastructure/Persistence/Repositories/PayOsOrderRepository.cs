using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class PayOsOrderRepository : IPayOsOrderRepository
    {
        private readonly AppDbContext _db;
        public PayOsOrderRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(PayOsOrder order, CancellationToken ct = default) =>
            await _db.PayOsOrders.AddAsync(order, ct);

        public Task<PayOsOrder?> GetByOrderCodeAsync(long orderCode, CancellationToken ct = default) =>
            _db.PayOsOrders.Include(o => o.User).FirstOrDefaultAsync(o => o.OrderCode == orderCode, ct);

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
