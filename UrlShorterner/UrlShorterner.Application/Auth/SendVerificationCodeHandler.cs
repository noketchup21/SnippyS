using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Auth
{
    public abstract record SendCodeResult
    {
        public record Success : SendCodeResult;
        public record TooSoon(int SecondsRemaining) : SendCodeResult;
        public record UserNotFound : SendCodeResult;
    }

    public class SendVerificationCodeHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeRepository _codeRepository;
        private readonly IEmailSender _emailSender;
        private const int CooldownSeconds = 60;
        private const int ExpiryMinutes = 15;

        public SendVerificationCodeHandler(IUserRepository userRepository, IVerificationCodeRepository codeRepository, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _emailSender = emailSender;
        }

        public async Task<SendCodeResult> HandleAsync(Guid userId, VerificationCodeType type, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user is null) return new SendCodeResult.UserNotFound();

            var latest = await _codeRepository.GetLatestAsync(userId, type, ct);
            if (latest is not null)
            {
                var secondsSince = (DateTimeOffset.UtcNow - latest.CreatedAt).TotalSeconds;
                if (secondsSince < CooldownSeconds)
                    return new SendCodeResult.TooSoon((int)(CooldownSeconds - secondsSince));
            }

            var code = Random.Shared.Next(100000, 999999).ToString();

            await _codeRepository.AddAsync(new VerificationCode
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = code,
                Type = type,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(ExpiryMinutes),
                CreatedAt = DateTimeOffset.UtcNow
            }, ct);
            await _codeRepository.SaveChangesAsync(ct);

            var subject = type == VerificationCodeType.EmailVerification
                ? "Verify your email"
                : "Reset your password";

            var heading = type == VerificationCodeType.EmailVerification
                ? "Verify your email"
                : "Reset your password";

            var bodyText = type == VerificationCodeType.EmailVerification
                ? "Enter this code to confirm your email address and finish setting up your account."
                : "Enter this code to choose a new password for your account.";

            var html = BuildVerificationEmailHtml(heading, bodyText, code, ExpiryMinutes);

            await _emailSender.SendAsync(user.Email, subject, html, ct);

            return new SendCodeResult.Success();
        }

        private static string BuildVerificationEmailHtml(string heading, string bodyText, string code, int expiryMinutes) => $$"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body style="margin:0; padding:0; background-color:#FFFDF5; font-family:'Plus Jakarta Sans', system-ui, sans-serif;">
  <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#FFFDF5; padding:40px 16px;">
    <tr>
      <td align="center">
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:420px;">

          <!-- Logo / brand -->
          <tr>
            <td align="center" style="padding-bottom:24px;">
              <table role="presentation" cellpadding="0" cellspacing="0">
                <tr>
                  <td style="width:36px; height:36px; background-color:#8B5CF6; border-radius:9999px; text-align:center; vertical-align:middle;">
                    <span style="color:#ffffff; font-weight:800; font-size:16px; line-height:36px;">🔗</span>
                  </td>
                  <td style="padding-left:8px;">
                    <span style="font-family:'Outfit', system-ui, sans-serif; font-weight:800; font-size:20px; color:#1E293B;">Snippy</span>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Card -->
          <tr>
            <td style="background-color:#ffffff; border:2px solid #1E293B; border-radius:24px; box-shadow:8px 8px 0px 0px #F472B6; padding:36px 32px; text-align:center;">
              <h1 style="font-family:'Outfit', system-ui, sans-serif; font-weight:800; font-size:22px; color:#1E293B; margin:0 0 12px;">
                {{heading}}
              </h1>
              <p style="font-size:14px; color:#64748B; margin:0 0 28px; line-height:1.5;">
                {{bodyText}}
              </p>

              <!-- Code block -->
              <table role="presentation" cellpadding="0" cellspacing="0" align="center" style="margin-bottom:28px;">
                <tr>
                  <td style="background-color:#F1F5F9; border:2px solid #1E293B; border-radius:16px; padding:16px 32px;">
                    <span style="font-family:'Outfit', system-ui, sans-serif; font-weight:800; font-size:32px; letter-spacing:8px; color:#1E293B;">
                      {{code}}
                    </span>
                  </td>
                </tr>
              </table>

              <p style="font-size:13px; color:#64748B; margin:0;">
                This code expires in {{expiryMinutes}} minutes.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td align="center" style="padding-top:24px;">
              <p style="font-size:12px; color:#94A3B8; margin:0;">
                Didn't request this? You can safely ignore this email.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>
""";
    }
}
