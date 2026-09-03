using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Application.Admin
{
    public abstract record BanUserResult
    {
        public record Success : BanUserResult;
        public record UserNotFound : BanUserResult;
    }

    public class BanUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminAuditLogger _auditLogger;

        public BanUserHandler(IUserRepository userRepository, IAdminAuditLogger auditLogger)
        {
            _userRepository = userRepository;
            _auditLogger = auditLogger;
        }

        public async Task<BanUserResult> HandleAsync(Guid adminUserId, Guid targetUserId, bool ban, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(targetUserId, ct);
            if (user is null)
            {
                return new BanUserResult.UserNotFound();
            }

            user.IsBanned = ban;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _userRepository.SaveChangesAsync(ct);

            await _auditLogger.LogAsync(adminUserId, ban ? "BAN_USER" : "UNBAN_USER", "User", targetUserId, ct: ct);

            return new BanUserResult.Success();
        }
    }
}
