using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard.Shared.CQRS.Application.Logic
{
    public class Mediator
    {
        private Assembly _assembly;
        private Dictionary<Type, object> _handlerTypesInstancesCache = new Dictionary<Type, object>();

        public Mediator(Assembly assembly)
        {
            _assembly = assembly;
        }



        public async Task<object> SendGeneric(object request)
        {
            // to do use service collection instead of assembly scanning
            // uwzglednic Irequest nie ma respsosne
            // 
            bool isCommand = request.GetType().GetInterfaces()
                .Any(i => !i.IsGenericType && i == typeof(IRequest));
            bool isQuery = !isCommand;

            if (isQuery) 
            { 
                return await SendQuery(request);
            }
            else
            {
                return await SendCommand(request);
            }

        }


        private async Task<object> SendCommand(object request)
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
            var handlerInstance = Activator.CreateInstance(handlerType);
            var methodInfo = handlerType.GetMethod("Handle");
            if (methodInfo == null)
            {
                throw new InvalidOperationException("Handler does not contain a Handle method");
            }

            var invoked = methodInfo.Invoke(handlerInstance, new object[] { request });
            if (invoked is Task task)
            {
                await task.ConfigureAwait(false);
                return task.IsCompletedSuccessfully;
            }

            return null;

        }

        private async Task<object> SendQuery(object request)
        {


            List<Type> handlerTypes = 
                GetQueryHandlers(request);


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


            // to do uwzglednic jak Irequest nie jest 


            var implementedInterfacesForRequest = request.GetType()
             .GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
             i.GetGenericTypeDefinition() == typeof(IRequest<>)
             && i.GetGenericArguments()[0] == responseType);



            if (implementedInterfacesForRequest == null)
            {
                throw new InvalidOperationException("reuest nie matchuje z responsem");
            }


            var handlerType = handlerTypes.First();
            var handlerInstance = Activator.CreateInstance(handlerType);
            var methodInfo = handlerType.GetMethod("Handle");
            if (methodInfo == null)
            {
                throw new InvalidOperationException("Handler does not contain a Handle method");
            }

            var invoked = methodInfo.Invoke(handlerInstance, new object[] { request });
            if (invoked is Task task)
            {
                await task.ConfigureAwait(false);
                var taskType = task.GetType();
                var resultProp = taskType.IsGenericType ? taskType.GetProperty("Result") : null;
                return resultProp?.GetValue(task);
            }

            return null;
        }

        private List<Type> GetQueryHandlers(object request)
        {

            List<Type> handlerTypes = _assembly.GetTypes()
                                              .Where(type => !type.IsInterface && !type.IsAbstract)
                                                .Where(type => type.GetInterfaces()
                                                .Any(i => i.IsGenericType &&
                                                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) &&
                                                 i.GetGenericArguments()[0] == request.GetType()))
                                                 .ToList();

            return handlerTypes;
        }
        

            private List<Type> GetCommandHandlers(object request)
            {
                List<Type> handlerTypes = _assembly.GetTypes()
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
