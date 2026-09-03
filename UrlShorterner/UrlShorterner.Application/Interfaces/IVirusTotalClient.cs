using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public enum VtCheckOutcome { Clean, Malicious, Pending, Error }

    public record VtCheckResult(VtCheckOutcome Outcome, string? Details = null);

    public interface IVirusTotalClient
    {
        Task<VtCheckResult> CheckUrlAsync(string url, CancellationToken ct = default);
    }
}
