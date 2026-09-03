using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Admin;

public record AdminStatsResponse(
    int TotalUsers,
    int PlusUsers,
    int BannedUsers,
    int TotalLinks,
    int ActiveLinks,
    int InactiveLinks,
    int CleanLinks,
    int PendingLinks,
    int MaliciousLinks,
    int UnresolvedLinks,
    int PendingReports);
