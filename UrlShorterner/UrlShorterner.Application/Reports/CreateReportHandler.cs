using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Reports
{
    public record CreateReportCommand(string ShortCodeOrUrl, Guid? ReportedByUserId, ReportReason Reason, string? Description);

    public abstract record CreateReportResult
    {
        public record Success(Guid Id) : CreateReportResult;
        public record LinkNotFound : CreateReportResult;
    }

    public class CreateReportHandler
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILinkRepository _linkRepository;

        public CreateReportHandler(IReportRepository reportRepository, ILinkRepository linkRepository)
        {
            _reportRepository = reportRepository;
            _linkRepository = linkRepository;
        }

        public async Task<CreateReportResult> HandleAsync(CreateReportCommand command, CancellationToken ct = default)
        {
            var input = command.ShortCodeOrUrl.Trim();
            // Extract short code if a full URL was pasted (e.g. https://short.ly/abc123 -> abc123)
            var shortCode = input.Contains('/') ? input.TrimEnd('/').Split('/').Last() : input;

            var link = await _linkRepository.GetByShortCodeAsync(shortCode, ct);
            if (link is null) return new CreateReportResult.LinkNotFound();

            var report = new Report
            {
                Id = Guid.NewGuid(),
                LinkId = link.Id,
                ReportedByUserId = command.ReportedByUserId,
                Reason = command.Reason,
                Description = command.Description,
                Status = ReportStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _reportRepository.AddAsync(report, ct);
            await _reportRepository.SaveChangesAsync(ct);

            return new CreateReportResult.Success(report.Id);
        }
    }
}
