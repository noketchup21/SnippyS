using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public record CreateLinkRequest(string OriginalUrl, string? CustomAlias = null, string? Password = null);
}
