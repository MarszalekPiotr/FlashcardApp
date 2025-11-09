// See https://aka.ms/new-console-template for more information
using FlashCard.Shared.CQRS.Application.Logic;
using medatortest.Command;
using medatortest.Query;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;


var assembly = Assembly.GetAssembly(typeof(Program));
IServiceCollection services = new ServiceCollection();
     //services.RegisterMediator(Assembly.GetExecutingAssembly());
    var serviceProvider = services.BuildServiceProvider();
  
    serviceProvider.CreateScope();
    var mediator = new Mediator(services, serviceProvider);
if (mediator == null)
    {
        throw new InvalidOperationException("Mediator nie zostal poprawnie zarejestrowany w kontenerze DI");
}

    var commandRequest = new RandomNymberCommandRequest
    {
        RequestTime = DateTime.Now
    };
    await mediator.SendCommand(commandRequest);
    var queryRequest = new RandomNymberQuery
    {
        RequestTime = DateTime.Now
    };
    var response = await mediator.SendQuery<RandomNumberResponse>(queryRequest);

 
var apiresponse = JsonConvert.SerializeObject(response);

    Console.WriteLine(apiresponse);



