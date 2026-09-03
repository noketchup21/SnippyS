using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Services
{
    public class PayOsPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly IPayOsOrderRepository _orderRepository;

        public PayOsPaymentService(HttpClient httpClient, IConfiguration configuration, IPayOsOrderRepository orderRepository)
        {
            _httpClient = httpClient;
            _clientId = configuration["PayOS:ClientId"] ?? throw new InvalidOperationException("PayOS:ClientId missing");
            _apiKey = configuration["PayOS:ApiKey"] ?? throw new InvalidOperationException("PayOS:ApiKey missing");
            _checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new InvalidOperationException("PayOS:ChecksumKey missing");
            _orderRepository = orderRepository;
        }

        public async Task<string> CreatePaymentLinkAsync(Guid userId, long amountVnd, CancellationToken ct = default)
        {
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 9_000_000_000; // stay within PayOS's numeric range, keep unique enough

            var description = "Snippy Plus - 1 thang";
            var returnUrl = "http://localhost:5173/account/upgrade/success";
            var cancelUrl = "http://localhost:5173/account/upgrade/cancelled";

            var signature = BuildSignature(orderCode, amountVnd, description, cancelUrl, returnUrl);

            var requestBody = new
            {
                orderCode,
                amount = amountVnd,
                description,
                returnUrl,
                cancelUrl,
                items = new[] { new { name = "Snippy Plus Subscription", quantity = 1, price = amountVnd } },
                signature
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v2/payment-requests")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-client-id", _clientId);
            request.Headers.Add("x-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var checkoutUrl = doc.RootElement.GetProperty("data").GetProperty("checkoutUrl").GetString()!;

            // Record the order BEFORE returning, so the webhook can resolve orderCode -> userId later
            await _orderRepository.AddAsync(new PayOsOrder
            {
                Id = Guid.NewGuid(),
                OrderCode = orderCode,
                UserId = userId,
                AmountVnd = amountVnd,
                Status = "Pending",
                CreatedAt = DateTimeOffset.UtcNow
            }, ct);
            await _orderRepository.SaveChangesAsync(ct);

            return checkoutUrl;
        }

        private string BuildSignature(long orderCode, long amount, string description, string cancelUrl, string returnUrl)
        {
            var raw = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            var keyBytes = Encoding.UTF8.GetBytes(_checksumKey);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(hash).ToLower();
        }
    }

}
