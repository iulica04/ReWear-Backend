using Application.Use_Cases.Commands.ClothingItemCommands.ClothingItemComandsValidator;
using Application.Use_Cases.Commands.OutfitCommands.OutfitCommandsValidator;
using Application.Use_Cases.Commands.UserCommands.UserComandsValidator;
using Application.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(
                cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(UpdateUserCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateClothingItemCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(UpdateClothingItemCommandValidator).Assembly); 
            services.AddValidatorsFromAssembly(typeof(CreateOutfitCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(UpdateOutfitCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(MatchOutfitItemsCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));



            return services;
        }
    }
}
