using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Auth;
using UrlShortener.Application.Interfaces;
using UrlShortener.Contracts.Auth;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").RequireRateLimiting("auth");

            group.MapPost("/register", async (
                RegisterRequest request,
                RegisterUserHandler handler,
                [FromServices] ICaptchaVerifier captchaVerifier,
                HttpContext http,
                CancellationToken ct) =>
            {
                if (!await captchaVerifier.VerifyAsync(request.CaptchaToken, ct))
                    return Results.Problem(detail: "Captcha verification failed.", statusCode: 400);

                var result = await handler.HandleAsync(new RegisterUserCommand(request.Email, request.Password), ct);
                return result switch
                {
                    RegisterUserResult.Success s => SetAuthCookieAndRespond(http, s.Token),
                    RegisterUserResult.EmailAlreadyExists => Results.Problem(
                        detail: "Email already registered.", statusCode: StatusCodes.Status409Conflict),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
                };
            });

            group.MapPost("/login", async (
                LoginRequest request,
                LoginUserHandler handler,
                [FromServices] ICaptchaVerifier captchaVerifier,
                HttpContext http,
                CancellationToken ct) =>
            {
                if (!await captchaVerifier.VerifyAsync(request.CaptchaToken, ct))
                    return Results.Problem(detail: "Captcha verification failed.", statusCode: 400);

                var result = await handler.HandleAsync(new LoginUserCommand(request.Email, request.Password), ct);
                return result switch
                {
                    LoginUserResult.Success s => SetAuthCookieAndRespond(http, s.Token),
                    LoginUserResult.InvalidCredentials => Results.Problem(
                        detail: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized),
                    LoginUserResult.UserBanned => Results.Problem(
                        detail: "This account has been banned.", statusCode: StatusCodes.Status403Forbidden),
                    LoginUserResult.EmailNotVerified n => Results.Problem(
                        detail: "Please verify your email before logging in.",
                        statusCode: StatusCodes.Status403Forbidden,
                        extensions: new Dictionary<string, object?> { ["emailNotVerified"] = true, ["userId"] = n.UserId }),
                    _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
                };
            });

            group.MapPost("/logout", (HttpContext http) =>
            {
                http.Response.Cookies.Delete("auth_token");
                return Results.NoContent();
            });

            group.MapGet("/me", (HttpContext http) =>
            {
                if (http.User.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                return Results.Ok(new
                {
                    id = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    email = http.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                    role = http.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
                    tier = http.User.FindFirst("tier")?.Value
                });
            }).RequireAuthorization();

            group.MapPost("/send-verification-code", async (
                [FromServices] SendVerificationCodeHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(sub, out var userId)) return Results.Unauthorized();

                var result = await handler.HandleAsync(userId, VerificationCodeType.EmailVerification, ct);
                return result switch
                {
                    SendCodeResult.Success => Results.Ok(new { message = "Verification code sent." }),
                    SendCodeResult.TooSoon t => Results.Problem(
                        detail: $"Please wait {t.SecondsRemaining} seconds before requesting another code.",
                        statusCode: 429),
                    _ => Results.Problem(statusCode: 500)
                };
            }).RequireAuthorization();

            group.MapPost("/verify-email", async (
                VerifyEmailRequest request,
                [FromServices] VerifyEmailHandler handler,
                HttpContext http,
                CancellationToken ct) =>
            {
                var sub = http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(sub, out var userId)) return Results.Unauthorized();

                var result = await handler.HandleAsync(userId, request.Code, ct);
                return result switch
                {
                    VerifyEmailResult.Success => Results.Ok(new { message = "Email verified." }),
                    VerifyEmailResult.InvalidOrExpiredCode => Results.Problem(
                        detail: "Invalid or expired code.", statusCode: 400),
                    _ => Results.Problem(statusCode: 500)
                };
            }).RequireAuthorization();

            group.MapPost("/forgot-password", async (
                ForgotPasswordRequest request,
                [FromServices] IUserRepository userRepository,
                [FromServices] SendVerificationCodeHandler handler,
                CancellationToken ct) =>
            {
                var user = await userRepository.GetByEmailAsync(request.Email, ct);
                // Always return 200 regardless of whether the email exists — don't leak account existence
                if (user is not null)
                {
                    await handler.HandleAsync(user.Id, VerificationCodeType.PasswordReset, ct);
                }
                return Results.Ok(new { message = "If that email exists, a reset code has been sent." });
            });

            group.MapPost("/reset-password", async (
                ResetPasswordRequest request,
                [FromServices] ResetPasswordHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request.Email, request.Code, request.NewPassword, ct);
                return result switch
                {
                    ResetPasswordResult.Success => Results.Ok(new { message = "Password reset successfully." }),
                    ResetPasswordResult.InvalidOrExpiredCode => Results.Problem(
                        detail: "Invalid or expired code.", statusCode: 400),
                    ResetPasswordResult.UserNotFound => Results.Problem(
                        detail: "Invalid request.", statusCode: 400),
                    _ => Results.Problem(statusCode: 500)
                };
            });

            group.MapPost("/send-verification-code-for/{userId:guid}", async (
                Guid userId,
                [FromServices] SendVerificationCodeHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(userId, VerificationCodeType.EmailVerification, ct);
                return result switch
                {
                    SendCodeResult.Success => Results.Ok(new { message = "Verification code sent." }),
                    SendCodeResult.TooSoon t => Results.Problem(
                        detail: $"Please wait {t.SecondsRemaining} seconds before requesting another code.", statusCode: 429),
                    _ => Results.Problem(statusCode: 500)
                };
            }); // no .RequireAuthorization() — this is the pre-login verification path

            group.MapPost("/verify-email-for/{userId:guid}", async (
                Guid userId,
                VerifyEmailRequest request,
                [FromServices] VerifyEmailHandler handler,
                [FromServices] IUserRepository userRepository,
                [FromServices] IJwtTokenGenerator jwtTokenGenerator,
                HttpContext http,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(userId, request.Code, ct);
                if (result is VerifyEmailResult.InvalidOrExpiredCode)
                    return Results.Problem(detail: "Invalid or expired code.", statusCode: 400);

                // Success — log them in immediately since they just proved email ownership
                var user = await userRepository.GetByIdAsync(userId, ct);
                if (user is null) return Results.Problem(statusCode: 500);

                var token = jwtTokenGenerator.GenerateToken(user);
                return SetAuthCookieAndRespond(http, token);
            });
        }

        private static IResult SetAuthCookieAndRespond(HttpContext http, string token)
        {
            http.Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,       // requires HTTPS
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });

            return Results.Ok();
        }
    }
}
