using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Interfaces
{
    public interface ICaptchaVerifier
    {
        Task<bool> VerifyAsync(string token, CancellationToken ct = default);
    }
}
