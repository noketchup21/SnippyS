using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Contracts.Auth
{
    public record ResetPasswordRequest(string Email, string Code, string NewPassword);
}
