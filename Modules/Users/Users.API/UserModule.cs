using Flashcard.Modules.Users.Application.Interfaces.Repositories;
using Flashcard.Modules.Users.Application.Logic.Abstract;
using FlashCard.Modules.Users.Infrastructure.Repositories;
using FlashCard.Shared.CQRS.Application.Logic;

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
