using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Auth
{
    public record RegisterUserCommand(string Email, string Password);

    public abstract record RegisterUserResult
    {
        public record Success(string Token) : RegisterUserResult;
        public record EmailAlreadyExists : RegisterUserResult;
    }
}
