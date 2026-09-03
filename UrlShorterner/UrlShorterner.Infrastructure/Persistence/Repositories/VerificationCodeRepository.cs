using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Infrastructure.Persistence.Repositories
{
    public class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly AppDbContext _db;
        public VerificationCodeRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(VerificationCode code, CancellationToken ct = default) =>
            await _db.VerificationCodes.AddAsync(code, ct);

        public Task<VerificationCode?> GetLatestAsync(Guid userId, VerificationCodeType type, CancellationToken ct = default) =>
            _db.VerificationCodes
                .Where(v => v.UserId == userId && v.Type == type)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync(ct);

        public Task<VerificationCode?> GetValidCodeAsync(Guid userId, string code, VerificationCodeType type, CancellationToken ct = default) =>
            _db.VerificationCodes.FirstOrDefaultAsync(v =>
                v.UserId == userId && v.Code == code && v.Type == type &&
                v.UsedAt == null && v.ExpiresAt > DateTimeOffset.UtcNow, ct);

        public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
