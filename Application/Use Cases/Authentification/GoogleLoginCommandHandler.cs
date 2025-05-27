using Application.Services;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<LoginResult>>
    {
        private readonly IGoogleAuthService googleAuthService;
        private readonly IUserRepository userRepository;

        public GoogleLoginCommandHandler(IGoogleAuthService googleAuthService, IUserRepository userRepository)
        {
            this.googleAuthService = googleAuthService;
            this.userRepository = userRepository;
        }
        public async Task<Result<LoginResult>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            // Verify Google token
            var payload = await googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
            if (!payload.IsSuccess)
            {
                return Result<LoginResult>.Failure("Google authentication failed.");
            }

            var email = payload.Data.Email;
            var googleId = payload.Data.Subject;

            var user = await userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                // If the user exists and is a local account, do not allow Google login
                if (user.LoginProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<LoginResult>.Failure("A local account with this email already exists. Please log in with your email and password.");
                }
                // If the user exists and is a Google account, continue login
            }
            else
            {
                // Create new user with Google
                var newUser = new Domain.Entities.User
                {
                    Email = email,
                    FirstName = payload.Data.GivenName,
                    LastName = payload.Data.FamilyName,
                    UserName = email,
                    PasswordHash = null,
                    Role = "User",
                    LoginProvider = "Google",
                    GoogleId = googleId,
                    ProfilePicture = payload.Data.Picture
                };

                var result = await userRepository.AddAsync(newUser);
                if (!result.IsSuccess)
                    return Result<LoginResult>.Failure("Failed to create user.");

                user = newUser;
            }

            // Generate JWT token for user
            var loginResult = await userRepository.LoginWithGoogle(email, googleId);

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                return Result<LoginResult>.Failure("Invalid credentials.");
            }

            return Result<LoginResult>.Success(loginResult);
        }

    }
}
