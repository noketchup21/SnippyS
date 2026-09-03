using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Reports
{
    public record CreateReportRequest(string ShortCodeOrUrl, string Reason, string? Description, string CaptchaToken);
}
