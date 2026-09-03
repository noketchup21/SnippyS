using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Links
{
    public record RecordClickCommand(string ShortCode, string? IpAddress, string? UserAgent, string? Referrer, string? Password);
}
