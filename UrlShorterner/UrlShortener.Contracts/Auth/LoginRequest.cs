using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Auth
{
    public record LoginRequest(string Email, string Password, string CaptchaToken);
}
