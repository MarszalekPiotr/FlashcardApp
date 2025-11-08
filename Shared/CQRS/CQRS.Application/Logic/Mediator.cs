using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlashCard.Shared.CQRS.Application.Logic
{
    public class Mediator
    {
  
        private IServiceCollection _services;
        private IServiceProvider _serviceProvider;
        public Mediator(IServiceCollection services,IServiceProvider serviceProvider)
        {           
            _services = services;
            _serviceProvider = serviceProvider;
        }
       
        public async Task SendCommand(IRequest request)
        {
            List<Type> handlerTypes =
                GetCommandHandlers(request);
            if (handlerTypes.Count == 0)
            {
                throw new InvalidOperationException("Nie znaleziono unikalnego handlera dla tego requestu");
            }
            else if (handlerTypes.Count > 1)
            {
                throw new InvalidOperationException("Znaleziono wiecej niz jeden handler dla tego requestu");
            }

            var handlerType = handlerTypes.First();
            _serviceProvider.CreateScope();
            var handlerInstance = _serviceProvider.GetService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");
            var result =  handleMethod?.Invoke(handlerInstance, new object[] { request }) as Task;

            if (handlerInstance == null)
            {
                throw new InvalidOperationException("Handler nie implementuje poprawnie interfejsu");
            }
        

        }

        public async Task<Tresponse> SendQuery<Tresponse>(IRequest<Tresponse> request) where Tresponse : class
        {
            List<Type> handlerTypes = 
                GetQueryHandlers<Tresponse>(request);


            if (handlerTypes.Count == 0)
            {
                throw new InvalidOperationException("Nie znaleziono unikalnego handlera dla tego requestu");
            }
            else if (handlerTypes.Count > 1)
            {
                throw new InvalidOperationException("Znaleziono wiecej niz jeden handler dla tego requestu");
            }

            var handler = handlerTypes.First();

            Type responseType = handler.GetInterfaces().Select(i => i.GetGenericArguments()[1]).FirstOrDefault();

            var implementedInterfacesForRequest = request.GetType()
             .GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
             i.GetGenericTypeDefinition() == typeof(IRequest<>)
             && i.GetGenericArguments()[0] == responseType);



            if (implementedInterfacesForRequest == null)
            {
                throw new InvalidOperationException("reuest nie matchuje z responsem");
            }
            
            var handlerType = handlerTypes.First();
            _serviceProvider.CreateScope();
            var handlerInstance = _serviceProvider.GetService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");
            var result = handleMethod?.Invoke(handlerInstance, new object[] { request });

            if (handlerInstance == null)
            {
                throw new InvalidOperationException("Handler nie implementuje poprawnie interfejsu");
            }

            return (result as Task<Tresponse>).Result;

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
