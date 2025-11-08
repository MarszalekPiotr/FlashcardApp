using Flashcard.Modules.Users.Application.Interfaces.Repositories;
using FlashCard.Modules.Users.Infrastructure.Repositories;

namespace FlashCard.Modules.Users.API
{
    public static class UserModule
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
            
            services.AddScoped<IUserRepository, UserRepository>();


            services.AddControllers()
           .AddApplicationPart(typeof(UserModule).Assembly);
            return services;
        }
    }
}
