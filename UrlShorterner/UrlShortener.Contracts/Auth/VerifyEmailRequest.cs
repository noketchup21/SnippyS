using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Auth
{
    public record VerifyEmailRequest(string Code);
}
