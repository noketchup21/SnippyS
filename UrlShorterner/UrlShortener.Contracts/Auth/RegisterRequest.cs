using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Auth
{
    public record RegisterRequest(string Email, string Password, string CaptchaToken);
}
