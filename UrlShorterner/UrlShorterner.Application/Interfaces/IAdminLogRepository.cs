using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces
{
    public interface IAdminLogRepository
    {
        Task<List<AdminLog>> GetRecentAsync(int count, CancellationToken ct = default);
    }
}
