using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public record LinkResponse(
        Guid Id,
        string ShortCode,
        string OriginalUrl,
        DateTimeOffset CreatedAt);
}
