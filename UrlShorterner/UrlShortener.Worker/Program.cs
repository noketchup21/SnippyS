using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Services;
using UrlShortener.Worker;
using UrlShortener.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>("urlshortenerdb", configureDbContextOptions: options =>
{
    options.UseSnakeCaseNamingConvention();
});
builder.AddRedisClient("redis");

builder.Services.AddHostedService<ClickAggregationJob>();

builder.Services.AddScoped<ILinkRepository, UrlShortener.Infrastructure.Persistence.Repositories.LinkRepository>();

builder.Services.AddHttpClient<IVirusTotalClient, VirusTotalClient>(client =>
{
    var apiKey = builder.Configuration["VirusTotal:ApiKey"]
        ?? throw new InvalidOperationException("VirusTotal:ApiKey is not configured.");
    client.DefaultRequestHeaders.Add("x-apikey", apiKey);
    client.Timeout = TimeSpan.FromSeconds(8);
});

builder.Services.AddHostedService<VirusTotalPollingJob>();

builder.Services.AddHttpClient<IAiSummaryService, GroqAiSummaryService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddHostedService<AiSummaryJob>();

var host = builder.Build();
host.Run();
