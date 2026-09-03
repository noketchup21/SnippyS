using System.Collections.Generic;
using System.Linq;

namespace UrlShortener.RedirectService.Rendering;

public static class RedirectPageRenderer
{
    private const string BaseStyles = """
        body { font-family: 'Plus Jakarta Sans', system-ui, sans-serif; background: #FFFDF5; color: #1E293B; display: flex; align-items: center; justify-content: center; min-height: 100vh; margin: 0; padding: 16px; box-sizing: border-box; }
        .wrap { width: 100%; max-width: 480px; }
        .brand { display: flex; align-items: center; justify-content: center; gap: 8px; margin-bottom: 20px; font-weight: 800; font-size: 18px; }
        .brand-icon { width: 32px; height: 32px; background: #8B5CF6; border-radius: 9999px; display:flex; align-items:center; justify-content:center; color:white; }
        .card { background: white; border: 2px solid #1E293B; border-radius: 24px; box-shadow: 8px 8px 0px 0px #F472B6; padding: 28px; text-align: center; }
        h1 { font-size: 18px; margin: 0 0 6px; }
        .error { color: #be185d; font-weight: 700; font-size: 13px; margin-bottom: 12px; }
        button { width: 100%; padding: 13px; background: #8B5CF6; color: white; border: 2px solid #1E293B; border-radius: 9999px; font-weight: 700; font-size: 15px; cursor: pointer; box-shadow: 4px 4px 0px 0px #1E293B; }
        """;

    private const string BrandHeader = "<div class=\"brand\"><span class=\"brand-icon\">🔗</span> SnippyS</div>";

    public static string RenderInterstitialPage(
        string shortCode, Guid linkId, string destination, string turnstileSiteKey,
        string? aiSummary, List<string>? aiKeyTopics, int? aiReadingMinutes,
        bool ownerIsPlus, bool aiSummaryAttempted, bool captchaError = false)
    {
        var summaryBlock = !string.IsNullOrEmpty(aiSummary)
            ? BuildAiSummaryBlock(aiSummary, aiKeyTopics, aiReadingMinutes)
            : ownerIsPlus && aiSummaryAttempted
                ? BuildAiUnavailableBlock()
                : !ownerIsPlus
                    ? BuildAiTeaserBlock()
                    : "";
        var errorBlock = captchaError ? "<div class=\"error\">Verification failed — please try again.</div>" : "";

        return $$"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Continue to your link</title>
  <script src="https://challenges.cloudflare.com/turnstile/v0/api.js" async defer></script>
  <style>
    {{BaseStyles}}
.dest { color: #64748B; font-size: 13px; margin: 0 0 16px; word-break: break-all; }
.ai-summary { background: #8B5CF6; border: 2px solid #1E293B; border-radius: 16px; padding: 16px; margin-bottom: 16px; text-align: left; }
.ai-summary-header { color: white; font-weight: 800; font-size: 12px; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 8px; }
.ai-summary-text { color: white; font-size: 14px; margin: 0 0 10px; line-height: 1.5; }
.ai-summary-meta { display: flex; flex-wrap: wrap; gap: 6px; align-items: center; }
.reading-time { color: rgba(255,255,255,0.85); font-size: 11px; font-weight: 700; }
.topic-chip { background: rgba(255,255,255,0.2); color: white; font-size: 11px; font-weight: 700; padding: 3px 10px; border-radius: 9999px; }
.ai-teaser { background: #F1F5F9; border: 2px dashed #8B5CF6; border-radius: 16px; padding: 14px 16px; margin-bottom: 16px; text-align: left; position: relative; }
.ai-teaser-badge { display: inline-block; background: #FBBF24; border: 2px solid #1E293B; border-radius: 9999px; padding: 2px 10px; font-size: 10px; font-weight: 800; margin-bottom: 6px; }
.ai-teaser-text { font-size: 13px; color: #475569; margin: 0; line-height: 1.5; }
.ai-teaser-text a { color: #8B5CF6; font-weight: 700; text-decoration: none; }
.ad-slot { background: #F1F5F9; border: 2px dashed #CBD5E1; border-radius: 16px; padding: 24px; margin-bottom: 20px; color: #94A3B8; font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; }
.captcha-wrap { display: flex; justify-content: center; margin-bottom: 16px; }
.ai-unavailable { display: flex; gap: 12px; align-items: flex-start; background: #F8FAFC; border: 2px solid #E2E8F0; border-radius: 16px; padding: 14px 16px; margin-bottom: 16px; text-align: left; }
.ai-unavailable-icon { font-size: 22px; line-height: 1; flex-shrink: 0; }
.ai-unavailable-title { font-weight: 800; font-size: 13px; color: #475569; margin: 0 0 3px; }
.ai-unavailable-text { font-size: 12px; color: #94A3B8; margin: 0; line-height: 1.5; }
button:disabled { opacity: 0.5; cursor: not-allowed; }
  </style>
</head>
<body>
  <div class="wrap">
    {{BrandHeader}}
    <div class="card">
      <h1>You're being redirected</h1>
      <p class="dest">→ {{destination}}</p>
      {{summaryBlock}}
      <div class="ad-slot">Advertisement space</div>
      {{errorBlock}}
      <form method="post" action="/{{shortCode}}/continue" id="continueForm">
        <input type="hidden" name="linkId" value="{{linkId}}" />
        <input type="hidden" name="destination" value="{{destination}}" />
        <div class="captcha-wrap">
          <div class="cf-turnstile" data-sitekey="{{turnstileSiteKey}}" data-callback="onCaptchaSuccess"></div>
        </div>
        <button type="submit" id="continueBtn" disabled>Continue to link</button>
      </form>
    </div>
  </div>
  <script>
    function onCaptchaSuccess() { document.getElementById('continueBtn').disabled = false; }
  </script>
</body>
</html>
""";
    }

    public static string RenderPasswordPromptPage(string shortCode, bool wrongPassword)
    {
        var errorBlock = wrongPassword ? "<div class=\"error\">Incorrect password — try again.</div>" : "";

        return $$"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Password required</title>
  <style>
    {{BaseStyles}}
    p.subtitle { color: #64748B; margin: 0 0 20px; font-size: 14px; }
    input { width: 100%; box-sizing: border-box; padding: 12px 16px; border: 2px solid #E2E8F0; border-radius: 16px; font-size: 16px; margin-bottom: 12px; }
  </style>
</head>
<body>
  <div class="wrap">
    {{BrandHeader}}
    <div class="card">
      <h1>🔒 This link is protected</h1>
      <p class="subtitle">Enter the password to continue.</p>
      {{errorBlock}}
      <form method="get" action="/{{shortCode}}">
        <input type="password" name="password" placeholder="Password" autofocus required />
        <button type="submit">Continue</button>
      </form>
    </div>
  </div>
</body>
</html>
""";
    }

    public static string RenderMessagePage(string message)
    {
        return $$"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <title>Link unavailable</title>
  <style>
    {{BaseStyles}}
  </style>
</head>
<body>
  <div class="wrap">
    {{BrandHeader}}
    <div class="card">
      <p style="margin:0;">{{message}}</p>
    </div>
  </div>
</body>
</html>
""";
    }

    private static string BuildAiSummaryBlock(string? aiSummary, List<string>? aiKeyTopics, int? aiReadingMinutes)
    {
        if (string.IsNullOrEmpty(aiSummary)) return "";

        var topicsHtml = aiKeyTopics is { Count: > 0 }
            ? string.Join("", aiKeyTopics.Select(t => $"<span class=\"topic-chip\">{t}</span>"))
            : "";
        var readingTimeHtml = aiReadingMinutes is > 0
            ? $"<span class=\"reading-time\">📖 ~{aiReadingMinutes} min read</span>"
            : "";

        return $$"""
            <div class="ai-summary">
              <div class="ai-summary-header">✨ AI Summary</div>
              <p class="ai-summary-text">{{aiSummary}}</p>
              <div class="ai-summary-meta">
                {{readingTimeHtml}}
                {{topicsHtml}}
              </div>
            </div>
            """;
    }

    private static string BuildAiTeaserBlock() => """
    <div class="ai-teaser">
      <span class="ai-teaser-badge">✨ NEW</span>
      <p class="ai-teaser-text">Plus links get an instant AI summary right here — <a href="/account/upgrade" target="_blank">see what Plus unlocks →</a></p>
    </div>
    """;

    private static string BuildAiUnavailableBlock() => """
    <div class="ai-unavailable">
      <div class="ai-unavailable-icon">✨</div>
      <div>
        <p class="ai-unavailable-title">AI summary not available</p>
        <p class="ai-unavailable-text">This site blocks automated readers, so our AI couldn't take a look. Nothing wrong with your link — some sites are just protective.</p>
      </div>
    </div>
    """;
}