using Application.Services;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageManagementService, ImageManagementService>();
            services.AddScoped<IClothingItemService, ClothingItemService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IClothingItemRepository, ClothingItemRepository>();
            services.AddScoped<IOLXServices, OLXService>();
            services.AddScoped<IOutfitRepository, OutfitRepository>();
            services.AddScoped<IOutfitService, OutfitService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IFavoriteOutfitRepository, FavoriteOutfitRepository>();

            services.AddHostedService<PasswordResetCleanupService>();
            services.AddHttpClient<IEmailService, EmailService>();
           
                

            return services;
        }
    }
}