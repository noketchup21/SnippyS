using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Auth
{
    public abstract record ResetPasswordResult
    {
        public record Success : ResetPasswordResult;
        public record InvalidOrExpiredCode : ResetPasswordResult;
        public record UserNotFound : ResetPasswordResult;
    }

    public class ResetPasswordHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeRepository _codeRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordHandler(IUserRepository userRepository, IVerificationCodeRepository codeRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResetPasswordResult> HandleAsync(string email, string code, string newPassword, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user is null) return new ResetPasswordResult.UserNotFound();

            var validCode = await _codeRepository.GetValidCodeAsync(user.Id, code, VerificationCodeType.PasswordReset, ct);
            if (validCode is null) return new ResetPasswordResult.InvalidOrExpiredCode();

            validCode.UsedAt = DateTimeOffset.UtcNow;
            user.PasswordHash = _passwordHasher.Hash(newPassword);
            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _codeRepository.SaveChangesAsync(ct);
            return new ResetPasswordResult.Success();
        }
    }
}
