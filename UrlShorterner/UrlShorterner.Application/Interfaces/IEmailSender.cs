using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }
}
