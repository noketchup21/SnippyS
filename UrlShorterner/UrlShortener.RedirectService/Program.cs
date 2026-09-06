using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Links;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Repositories;
using UrlShortener.Infrastructure.Services;
using UrlShortener.RedirectService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("urlshortenerdb", configureDbContextOptions: options =>
{
    options.UseSnakeCaseNamingConvention();
});

builder.AddRedisClient("redis");

builder.Services.AddProblemDetails();
builder.Services.AddHttpClient<ICaptchaVerifier, TurnstileCaptchaVerifier>();

// Only register what redirects actually need — nothing about auth, VT, admin, etc.
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddSingleton<IClickEventPublisher, RedisClickEventPublisher>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<RedirectHandler>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("redirects", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseRateLimiter();
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.MapRedirectEndpoints();

app.Run();