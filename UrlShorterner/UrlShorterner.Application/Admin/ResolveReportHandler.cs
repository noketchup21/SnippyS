using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Admin
{
    public abstract record ResolveReportResult
    {
        public record Success : ResolveReportResult;
        public record ReportNotFound : ResolveReportResult;
    }

    public class ResolveReportHandler
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILinkRepository _linkRepository;
        private readonly IAdminAuditLogger _auditLogger;

        public ResolveReportHandler(IReportRepository reportRepository, ILinkRepository linkRepository, IAdminAuditLogger auditLogger)
        {
            _reportRepository = reportRepository;
            _linkRepository = linkRepository;
            _auditLogger = auditLogger;
        }

        public async Task<ResolveReportResult> HandleAsync(Guid adminUserId, Guid reportId, bool deactivateLink, CancellationToken ct = default)
        {
            var report = await _reportRepository.GetByIdAsync(reportId, ct);
            if(report is null)
            {
                return new ResolveReportResult.ReportNotFound();
            }

            report.Status = deactivateLink ? ReportStatus.ActionTaken : ReportStatus.Dismissed;
            report.ResolvedByAdminId = adminUserId;
            report.ResolvedAt = DateTimeOffset.UtcNow;

            if(deactivateLink)
            {
                report.Link.IsActive = false;
            }
            await _reportRepository.SaveChangesAsync(ct);

            await _auditLogger.LogAsync(
                adminUserId,
                deactivateLink ? "DEACTIVATE_LINK_VIA_REPORT" : "DISMISS_REPORT",
                "Report",
                reportId,
                ct: ct);

            return new ResolveReportResult.Success();
        }
    }
}
