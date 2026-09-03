using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Infrastructure.Services
{
    public class TurnstileCaptchaVerifier : ICaptchaVerifier
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public TurnstileCaptchaVerifier(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Turnstile:SecretKey"]
                ?? throw new InvalidOperationException("Turnstile:SecretKey is not configured.");
        }

        public async Task<bool> VerifyAsync(string token, CancellationToken ct = default)
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", _secretKey),
            new KeyValuePair<string, string>("response", token),
        });

            var response = await _httpClient.PostAsync(
                "https://challenges.cloudflare.com/turnstile/v0/siteverify", content, ct);

            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("success", out var success) && success.GetBoolean();
        }
    }
}
