using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces
{
    public interface IPayOsOrderRepository
    {
        Task AddAsync(PayOsOrder order, CancellationToken ct = default);
        Task<PayOsOrder?> GetByOrderCodeAsync(long orderCode, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
