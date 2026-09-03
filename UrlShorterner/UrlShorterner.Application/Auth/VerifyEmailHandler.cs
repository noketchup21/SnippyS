using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Auth
{
    public abstract record VerifyEmailResult
    {
        public record Success : VerifyEmailResult;
        public record InvalidOrExpiredCode : VerifyEmailResult;
    }

    public class VerifyEmailHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeRepository _codeRepository;

        public VerifyEmailHandler(IUserRepository userRepository, IVerificationCodeRepository codeRepository)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
        }

        public async Task<VerifyEmailResult> HandleAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var validCode = await _codeRepository.GetValidCodeAsync(userId, code, VerificationCodeType.EmailVerification, ct);
            if (validCode is null) return new VerifyEmailResult.InvalidOrExpiredCode();

            validCode.UsedAt = DateTimeOffset.UtcNow;

            var user = await _userRepository.GetByIdAsync(userId, ct);
            user!.EmailVerified = true;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _codeRepository.SaveChangesAsync(ct);
            return new VerifyEmailResult.Success();
        }
    }
}
