using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Contracts.Links;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Links
{
    public class CreateShortLinkHandler
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IQuotaService _quotaService;
        private readonly IVirusTotalClient _virusTotalClient;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAiSummaryQueue _aiSummaryQueue;

        public CreateShortLinkHandler(ILinkRepository linkRepository, IQuotaService quotaService, IVirusTotalClient virusTotalClient, IPasswordHasher passwordHasher, IAiSummaryQueue aiSummaryQueue)
        {
            _linkRepository = linkRepository;
            _quotaService = quotaService;
            _virusTotalClient = virusTotalClient;
            _passwordHasher = passwordHasher;
            _aiSummaryQueue = aiSummaryQueue;
        }

        public async Task<CreateShortLinkResult> HandleAsync(CreateShortLinkCommand command, CancellationToken ct = default)
        {
            //validating the URL before creating a short link
            if (!Uri.TryCreate(command.OriginalUrl, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return new CreateShortLinkResult.InvalidUrl("URL must be a valid absolute http/https URL.");
            }

            //custom alias and password are Plus-only features
            if (command.CustomAlias is not null && !command.IsPlus)
            {
                return new CreateShortLinkResult.PlusFeatureRequired("Custom aliases require a Plus subscription.");
            }

            if (command.Password is not null && !command.IsPlus)
            {
                return new CreateShortLinkResult.PlusFeatureRequired("Password-protected links require a Plus subscription.");
            }

            //resolve short code: custom alias if provided and valid, otherwise generate one
            string shortCode;
            if (command.CustomAlias is not null)
            {
                if (!IsValidAlias(command.CustomAlias))
                {
                    return new CreateShortLinkResult.InvalidUrl("Custom alias must be 3-16 alphanumeric characters, hyphens, or underscores.");
                }

                if (await _linkRepository.ShortCodeExistsAsync(command.CustomAlias, ct))
                {
                    return new CreateShortLinkResult.AliasTaken();
                }

                shortCode = command.CustomAlias;
            }
            else
            {
                shortCode = await GenerateUniquenessCodeAsync(ct);
            }

            //virus total scan to check if the URL is malicious
            var vtResult = await _virusTotalClient.CheckUrlAsync(command.OriginalUrl, ct);
            if (vtResult.Outcome == VtCheckOutcome.Malicious)
            {
                return new CreateShortLinkResult.MaliciousUrl(vtResult.Details ?? "URL flagged as malicious.");
            }

            //handle quota for anonymous users and standard users, plus users
            var allowed = command.UserId is null
                ? await _quotaService.TryConsumeAnonymousQuotaAsync(command.Fingerprint!, ct)
                : command.IsPlus
                    ? true // Plus = unlimited
                    : await _quotaService.TryConsumeStandardQuotaAsync(command.UserId.Value, ct);

            if (!allowed)
            {
                return new CreateShortLinkResult.QuotaExceeded();
            }

            var link = new Link
            {
                Id = Guid.NewGuid(),
                ShortCode = shortCode,
                OriginalUrl = command.OriginalUrl,
                UserId = command.UserId,
                IsCustomAlias = command.CustomAlias is not null,
                PasswordHash = command.Password is not null ? _passwordHasher.Hash(command.Password) : null,
                VtStatus = vtResult.Outcome == VtCheckOutcome.Clean ? VtStatus.Clean : VtStatus.Pending,
                VtCheckedAt = vtResult.Outcome == VtCheckOutcome.Clean ? DateTimeOffset.UtcNow : null,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _linkRepository.AddAsync(link, ct);
            await _linkRepository.SaveChangesAsync(ct);

            if (command.IsPlus)
            {
                await _aiSummaryQueue.EnqueueAsync(link.Id, ct); // fire-and-forget signal, doesn't block response
            }

            return new CreateShortLinkResult.Success(link.Id, link.ShortCode, link.OriginalUrl, link.CreatedAt);
        }

        private static bool IsValidAlias(string alias) =>
            alias.Length is >= 3 and <= 16 &&
            alias.All(c => char.IsLetterOrDigit(c) || c is '-' or '_');

        private async Task<string> GenerateUniquenessCodeAsync(CancellationToken ct)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = Random.Shared;

            for(var attempt = 0; attempt < 5; attempt++)
            {
                var code = new string(Enumerable.Range(0, 7).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                if (!await _linkRepository.ShortCodeExistsAsync(code, ct))
                    return code;
            }
            throw new InvalidOperationException("Failed to generate a unique short code after 5 attempts.");
        }
    }
}
