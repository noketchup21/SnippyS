using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using UrlShortener.Application.Admin;
using UrlShortener.Application.Auth;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Links;
using UrlShortener.Application.Reports;
using UrlShortener.Application.Subscriptions;
using UrlShortener.Endpoints;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Repositories;
using UrlShortener.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("urlshortenerdb", configureDbContextOptions: options =>
{
    options.UseSnakeCaseNamingConvention();
});

builder.AddRedisClient("redis");

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "UrlShortener";
var apiKey = builder.Configuration["VirusTotal:ApiKey"]
    ?? throw new InvalidOperationException("VirusTotal:ApiKey is not configured.");
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"]
    ?? throw new InvalidOperationException("Stripe:SecretKey is not configured.");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddSingleton<IQuotaService, RedisQuotaService>();
builder.Services.AddScoped<CreateShortLinkHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAdminAuditLogger, AdminAuditLogger>();
builder.Services.AddScoped<IAdminLogRepository, AdminLogRepository>();
builder.Services.AddScoped<BanUserHandler>();
builder.Services.AddScoped<ResolveReportHandler>();
builder.Services.AddScoped<CreateReportHandler>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();
builder.Services.AddScoped<StripePaymentService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<HandleStripeWebhookHandler>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
builder.Services.AddScoped<SendVerificationCodeHandler>();
builder.Services.AddScoped<VerifyEmailHandler>();
builder.Services.AddScoped<ResetPasswordHandler>();
builder.Services.AddHttpClient<ICaptchaVerifier, TurnstileCaptchaVerifier>();
builder.Services.AddSingleton<IAiSummaryQueue, RedisAiSummaryQueue>();
builder.Services.AddHttpClient<PayOsPaymentService>();
builder.Services.AddScoped<IPaymentService, CompositePaymentService>();
builder.Services.AddScoped<IPayOsOrderRepository, PayOsOrderRepository>();
builder.Services.AddScoped<HandlePayOsWebhookHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("auth_token", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("PlusFeature", policy => policy.RequireClaim("tier", "Plus"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddHttpClient<IVirusTotalClient, VirusTotalClient>(client =>
{
    var apiKey = builder.Configuration["VirusTotal:ApiKey"]
        ?? throw new InvalidOperationException("VirusTotal:ApiKey is not configured.");
    client.DefaultRequestHeaders.Add("x-apikey", apiKey);
    client.Timeout = TimeSpan.FromSeconds(8);
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("general", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(); // uses AddProblemDetails() config, hides stack traces in prod
}

//map endpoints
app.MapLinkEndpoints();
app.MapAuthEndpoints();
app.MapAnalyticsEndpoints();
app.MapReportEndpoints();
app.MapAdminEndpoints();
app.MapSubscriptionEndpoints();
app.MapWebhookEndpoints();

app.Run();
