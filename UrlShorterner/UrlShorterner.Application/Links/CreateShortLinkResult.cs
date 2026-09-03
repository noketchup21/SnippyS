using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public abstract record CreateShortLinkResult
    {
        public record Success(Guid Id, string ShortCode, string OriginalUrl, DateTimeOffset CreatedAt) : CreateShortLinkResult;
        public record QuotaExceeded : CreateShortLinkResult;
        public record InvalidUrl(string Reason) : CreateShortLinkResult;
        public record MaliciousUrl(string Reason) : CreateShortLinkResult;
        public record PlusFeatureRequired(string Reason) : CreateShortLinkResult;
        public record AliasTaken : CreateShortLinkResult;
    }
}
