using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Auth
{
    public record ForgotPasswordRequest(string Email);
}
