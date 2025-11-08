using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlashCard.Shared.CQRS.Application.Logic
{
    public class Mediator
    {
        private IServiceCollection _services;
        private IServiceProvider _serviceProvider;
        public Mediator(IServiceCollection services, IServiceProvider serviceProvider)
        {
            _services = services;
            _serviceProvider = serviceProvider;
        }

        public async Task SendCommand(IRequest request)
        {
            List<Type> handlerTypes = GetCommandHandlers(request);
            if (handlerTypes.Count == 0)
                throw new InvalidOperationException("Nie znaleziono unikalnego handlera dla tego requestu");
            if (handlerTypes.Count > 1)
                throw new InvalidOperationException("Znaleziono wiecej niz jeden handler dla tego requestu");

            var handlerType = handlerTypes.First();

            // Utwórz scope i resolve w jego ramach
            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var handlerInstance = sp.GetService(handlerType);
            if (handlerInstance == null)
                throw new InvalidOperationException("Handler nie implementuje poprawnie interfejsu lub nie jest zarejestrowany");

            var handleMethod = handlerType.GetMethod("Handle", new[] { request.GetType() }) ?? handlerType.GetMethod("Handle");
            if (handleMethod == null)
                throw new InvalidOperationException("Metoda Handle nieznaleziona w handlerze");

            var taskObj = handleMethod.Invoke(handlerInstance, new object[] { request });
            if (taskObj is Task task)
            {
                await task.ConfigureAwait(false);
                return;
            }

            throw new InvalidOperationException("Handle nie zwrócił Task");
        }

        public async Task<Tresponse> SendQuery<Tresponse>(IRequest<Tresponse> request) where Tresponse : class
        {
            List<Type> handlerTypes = GetQueryHandlers<Tresponse>(request);

            if (handlerTypes.Count == 0)
                throw new InvalidOperationException("Nie znaleziono unikalnego handlera dla tego requestu");
            if (handlerTypes.Count > 1)
                throw new InvalidOperationException("Znaleziono wiecej niz jeden handler dla tego requestu");

            var handler = handlerTypes.First();

            Type responseType = handler.GetInterfaces().Select(i => i.GetGenericArguments()[1]).FirstOrDefault();

            var implementedInterfacesForRequest = request.GetType()
                .GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequest<>)
                && i.GetGenericArguments()[0] == responseType);

            if (implementedInterfacesForRequest == null)
                throw new InvalidOperationException("request nie matchuje z responsem");

            var handlerType = handlerTypes.First();

            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var handlerInstance = sp.GetService(handlerType);
            if (handlerInstance == null)
                throw new InvalidOperationException("Handler nie implementuje poprawnie interfejsu lub nie jest zarejestrowany");

            var handleMethod = handlerType.GetMethod("Handle", new[] { request.GetType() }) ?? handlerType.GetMethod("Handle");
            if (handleMethod == null)
                throw new InvalidOperationException("Metoda Handle nieznaleziona w handlerze");

            var taskObj = handleMethod.Invoke(handlerInstance, new object[] { request });

            if (taskObj is Task<Tresponse> typedTask)
            {
                return await typedTask.ConfigureAwait(false);
            }

            if (taskObj is Task task)
            {
                await task.ConfigureAwait(false);
                var resultProperty = taskObj.GetType().GetProperty("Result");
                if (resultProperty != null)
                {
                    return resultProperty.GetValue(taskObj) as Tresponse;
                }
                return null;
            }

            throw new InvalidOperationException("Handle nie zwrócił Task lub Task<T>");
        }

        private List<Type> GetQueryHandlers<Tres>(IRequest<Tres> request) where Tres : class
        {
            List<Type> handlerTypes = _services.Select(sd => sd.ImplementationType ?? sd.ServiceType)
                                                .Distinct()
                                                .Where(type => !type.IsInterface && !type.IsAbstract)
                                                .Where(type => type.GetInterfaces()
                                                .Any(i => i.IsGenericType &&
                                                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) &&
                                                 i.GetGenericArguments()[0] == request.GetType()
                                                 && i.GetGenericArguments()[1] == typeof(Tres)))
                                                 .ToList();

            return handlerTypes;
        }

        private List<Type> GetCommandHandlers(IRequest request)
        {
            List<Type> handlerTypes = _services.Select(sd => sd.ImplementationType ?? sd.ServiceType)
                                            .Distinct()
                                                .Where(type => !type.IsInterface && !type.IsAbstract)
                                                .Where(type => type.GetInterfaces()
                                                .Any(i => i.IsGenericType &&
                                                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) &&
                                                 i.GetGenericArguments()[0] == request.GetType()))
                                                 .ToList();
            return handlerTypes;
        }
    }
}
