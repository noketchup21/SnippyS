using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Application.Auth
{
    public class LoginUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginUserResult> HandleAsync(LoginUserCommand command, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return new LoginUserResult.InvalidCredentials();

            if (user.IsBanned)
                return new LoginUserResult.UserBanned();

            if (!user.EmailVerified)
                return new LoginUserResult.EmailNotVerified(user.Id);

            var token = _jwtTokenGenerator.GenerateToken(user);
            return new LoginUserResult.Success(token);
        }
    }
}
