using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Auth
{
    public record LoginUserCommand(string Email, string Password);

    public abstract record LoginUserResult
    {
        public record Success(string Token) : LoginUserResult;
        public record InvalidCredentials : LoginUserResult;
        public record UserBanned : LoginUserResult;
        public record EmailNotVerified(Guid UserId) : LoginUserResult;
    }
}
