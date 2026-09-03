using System.Text;
using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class GroqAiSummaryService : IAiSummaryService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GroqAiSummaryService> _logger;
    private const string GroqEndpoint = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model = "llama-3.3-70b-versatile";

    public GroqAiSummaryService(HttpClient httpClient, IConfiguration configuration, ILogger<GroqAiSummaryService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq:ApiKey is not configured.");
        _logger = logger;
    }

    public async Task<AiSummaryResult?> GenerateSummaryAsync(string url, CancellationToken ct = default)
    {
        try
        {
            var pageText = await FetchReadableTextAsync(url, ct);
            if (string.IsNullOrWhiteSpace(pageText))
                return null;

            if (pageText.Length > 8000)
                pageText = pageText[..8000];

            var wordCount = pageText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var estimatedReadingMinutes = Math.Max(1, wordCount / 200);

            var requestBody = new
            {
                model = Model,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = "You analyze webpage content and respond ONLY with valid JSON, no markdown, no code fences, in exactly this shape: {\"summary\": \"one concise sentence\", \"keyTopics\": [\"topic1\", \"topic2\", \"topic3\"], \"keywords\": [\"kw1\", \"kw2\", \"kw3\", \"kw4\", \"kw5\"]}"
                    },
                    new
                    {
                        role = "user",
                        content = $"Analyze this webpage content:\n\n{pageText}"
                    }
                },
                temperature = 0.3,
                response_format = new { type = "json_object" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, GroqEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Groq API returned {StatusCode} for {Url}: {Body}", response.StatusCode, url, errorBody);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var groqText = ExtractGroqResponseText(json);
            if (groqText is null) return null;

            var parsed = ParseSummaryJson(groqText);
            if (parsed is null) return null;

            return parsed with { EstimatedReadingMinutes = estimatedReadingMinutes };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI summary generation failed for {Url}", url);
            return null;
        }
    }

    private async Task<string?> FetchReadableTextAsync(string url, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            using var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch page content for {Url}: {StatusCode}", url, response.StatusCode);
                return null;
            }

            var html = await response.Content.ReadAsStringAsync(ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//nav|//footer|//header");
            if (nodesToRemove is not null)
                foreach (var node in nodesToRemove) node.Remove();

            var text = doc.DocumentNode.InnerText;
            return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception fetching page content for {Url}", url);
            return null;
        }
    }

    private static string? ExtractGroqResponseText(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }

    private static AiSummaryResult? ParseSummaryJson(string text)
    {
        try
        {
            var cleaned = text.Trim().Trim('`').Replace("json", "", StringComparison.OrdinalIgnoreCase).Trim();

            using var doc = JsonDocument.Parse(cleaned);
            var root = doc.RootElement;

            var summary = root.GetProperty("summary").GetString() ?? "";
            var keyTopics = root.GetProperty("keyTopics").EnumerateArray().Select(e => e.GetString() ?? "").ToList();
            var keywords = root.GetProperty("keywords").EnumerateArray().Select(e => e.GetString() ?? "").ToList();

            return new AiSummaryResult(summary, keyTopics, keywords, 0);
        }
        catch
        {
            return null;
        }
    }
}