using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class PasswordResetCleanupService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public PasswordResetCleanupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var passwordResetCodeRepository = scope.ServiceProvider.GetRequiredService<IPasswordResetCodeRepository>();
                    var expirationTime = DateTime.UtcNow.AddMinutes(-15);

                    var expiredCodes = await passwordResetCodeRepository.GetExpiredCodesAsync(expirationTime);

                    if (expiredCodes.Any())
                    {
                        await passwordResetCodeRepository.DeleteExpiredCodesAsync(expiredCodes);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}