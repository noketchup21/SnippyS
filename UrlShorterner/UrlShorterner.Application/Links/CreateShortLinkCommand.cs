using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public record CreateShortLinkCommand(
        string OriginalUrl,
        Guid? UserId,        // null = anonymous
        string? Fingerprint,
        bool IsPlus = false,
        string? CustomAlias = null,
        string? Password = null); // required when UserId is null
}
