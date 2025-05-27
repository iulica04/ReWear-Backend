using Application.Services;
using Domain.Common;
using Google.Apis.Auth;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public async Task<Result<GoogleJsonWebSignature.Payload>> VerifyGoogleTokenAsync(string token)
        {
            try
            {

                var payload = await GoogleJsonWebSignature.ValidateAsync(token);
                return Result<GoogleJsonWebSignature.Payload>.Success(payload);
            }
            catch (InvalidJwtException ex)
            {

                return Result<GoogleJsonWebSignature.Payload>.Failure($"Token invalid: {ex.Message}");
            }
            catch (Exception ex)
            {

                return Result<GoogleJsonWebSignature.Payload>.Failure($"Eroare la validarea tokenului: {ex.Message}");
            }
        }
    }
}
