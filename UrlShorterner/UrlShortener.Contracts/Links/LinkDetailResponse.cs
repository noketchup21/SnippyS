using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links;

public record LinkDetailResponse(
    Guid Id,
    string ShortCode,
    string OriginalUrl,
    bool IsActive,
    bool IsCustomAlias,
    bool HasPassword,
    string VtStatus,
    DateTimeOffset CreatedAt);
