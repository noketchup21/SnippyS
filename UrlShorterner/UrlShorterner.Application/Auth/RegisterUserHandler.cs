using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Auth
{
    public class RegisterUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<RegisterUserResult> HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
        {
            var existing = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (existing is not null)
            {
                return new RegisterUserResult.EmailAlreadyExists();
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                PasswordHash = _passwordHasher.Hash(command.Password),
                Role = UserRole.User,
                Tier = SubscriptionTier.Standard, // new accounts always start Standard
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            var token = _jwtTokenGenerator.GenerateToken(user);
            return new RegisterUserResult.Success(token);
        }
    }
}
