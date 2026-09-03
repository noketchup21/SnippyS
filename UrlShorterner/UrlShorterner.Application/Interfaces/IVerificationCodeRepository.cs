using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task AddAsync(VerificationCode code, CancellationToken ct = default);
        Task<VerificationCode?> GetLatestAsync(Guid userId, VerificationCodeType type, CancellationToken ct = default);
        Task<VerificationCode?> GetValidCodeAsync(Guid userId, string code, VerificationCodeType type, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
