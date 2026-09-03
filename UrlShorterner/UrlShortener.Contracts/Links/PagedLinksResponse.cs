using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Links
{
    public record PagedLinksResponse(List<LinkSummaryResponse> Items, int TotalCount, int Page, int PageSize);
}
