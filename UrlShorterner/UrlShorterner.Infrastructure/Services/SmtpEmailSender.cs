using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Smtp:FromName"] ?? "UrlShortener",
                _configuration["Smtp:FromEmail"] ?? "noreply@urlshortener.dev"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Smtp:Host"],
                int.Parse(_configuration["Smtp:Port"] ?? "587"),
                MailKit.Security.SecureSocketOptions.StartTls,
                ct);
            await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"], ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception ex)
        {
            // Email delivery is best-effort — don't let a provider-side failure
            // (rate limit, account restriction, transient SMTP error) block the
            // caller's actual operation (register/login/verify already succeeded
            // at the DB level by this point).
            _logger.LogError(ex, "Failed to send email to {Email} with subject {Subject}", toEmail, subject);
        }
    }
}