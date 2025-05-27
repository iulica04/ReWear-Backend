using Domain.Common;
using Google.Apis.Auth;

namespace Application.Services
{
    public interface IGoogleAuthService
    {
        Task<Result<GoogleJsonWebSignature.Payload>> VerifyGoogleTokenAsync(string token);
    }
}
