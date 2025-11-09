using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard.Shared.CQRS.Application.Logic
{
    public static class MediatorExtension
    {
        public static IServiceCollection RegisterMediator(this IServiceCollection services, Assembly[] assemblies)
        {
             

            foreach (var assembly in assemblies)
            {
                var assemlbytypes = assembly.GetTypes();

                var commandHandlerTypes = assembly.GetTypes()
                                                      .Where(type => !type.IsInterface && !type.IsAbstract)
                                                        .Where(type => type.GetInterfaces()
                                                        .Any(i => i.IsGenericType &&
                                                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) &&
                                                         // sprawdzamy czy TReq implementuje non-generic IRequest
                                                         typeof(IRequest).IsAssignableFrom(i.GetGenericArguments()[0])))
                                                         .ToList();

                var queryHandlerTypes = assembly.GetTypes()
                                                      .Where(type => !type.IsInterface && !type.IsAbstract)
                                                        .Where(type => type.GetInterfaces()
                                                        .Any(i => i.IsGenericType &&
                                                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) &&
                                                         // sprawdzamy czy TReq implementuje IRequest<...> — porównujemy definicję generyczną
                                                         i.GetGenericArguments()[0].GetInterfaces()
                                                            .Any(j => j.IsGenericType && j.GetGenericTypeDefinition() == typeof(IRequest<>))))
                                                         .ToList();

                var allHandlerTypes = commandHandlerTypes.Concat(queryHandlerTypes);

                foreach (var handlerType in allHandlerTypes)
                {
                    services.AddScoped(handlerType);
                }
            }
           

            services.AddSingleton<Mediator>(sp => new Mediator(services, sp));


            return services;


        }
    }
}
