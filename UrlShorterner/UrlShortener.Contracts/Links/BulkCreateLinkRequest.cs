using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public record BulkCreateLinkRequest(List<string> Urls);
    public record BulkCreateLinkResultItem(string OriginalUrl, bool Success, string? ShortCode, string? Error);
}
