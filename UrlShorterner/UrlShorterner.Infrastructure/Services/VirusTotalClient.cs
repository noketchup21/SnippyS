using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class VirusTotalClient : IVirusTotalClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.virustotal.com/api/v3";

        public VirusTotalClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<VtCheckResult> CheckUrlAsync(string url, CancellationToken ct = default)
        {
            try
            {
                // VT identifies URLs by base64url(url) without padding for GET lookups
                var urlId = Convert.ToBase64String(Encoding.UTF8.GetBytes(url))
                    .TrimEnd('=').Replace('+', '-').Replace('/', '_');
                var lookupResponse = await _httpClient.GetAsync($"{BaseUrl}/urls/{urlId}", ct);

                if (lookupResponse.IsSuccessStatusCode)
                {
                    var json = await lookupResponse.Content.ReadAsStringAsync(ct);
                    return ParseAnalysisStats(json);
                }

                if (lookupResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // VT has no prior record — submit for scanning, return Pending
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("url", url) });
                    var submitResponse = await _httpClient.PostAsync($"{BaseUrl}/urls", content, ct);
                    submitResponse.EnsureSuccessStatusCode();
                    return new VtCheckResult(VtCheckOutcome.Pending, "Submitted for analysis.");
                }

                return new VtCheckResult(VtCheckOutcome.Error, $"VirusTotal returned {lookupResponse.StatusCode}");
            }
            catch (Exception ex)
            {
                // TEMP: log the real exception to diagnose parsing failures
                Console.WriteLine($"VT check failed for {url}: {ex}");
                // Fail-open to Pending rather than blocking link creation entirely on VT outages
                return new VtCheckResult(VtCheckOutcome.Pending, $"VT check failed: {ex.Message}");
            }
        }

        private static VtCheckResult ParseAnalysisStats(string json)
        {
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("attributes", out var attributes) ||
                !attributes.TryGetProperty("last_analysis_stats", out var stats))
            {
                return new VtCheckResult(VtCheckOutcome.Pending, "No analysis stats available yet.");
            }

            var malicious = stats.TryGetProperty("malicious", out var m) ? m.GetInt32() : 0;
            var suspicious = stats.TryGetProperty("suspicious", out var s) ? s.GetInt32() : 0;

            return malicious > 0 || suspicious > 0
                ? new VtCheckResult(VtCheckOutcome.Malicious, $"{malicious} malicious, {suspicious} suspicious detections")
                : new VtCheckResult(VtCheckOutcome.Clean);
        }
    }
}
