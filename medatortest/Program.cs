// See https://aka.ms/new-console-template for more information
using FlashCard.Shared.CQRS.Application.Logic;
using medatortest.Command;
using medatortest.Query;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;


if (false)
{



    var mediator = new FlashCard.Shared.CQRS.Application.Logic.Mediator(Assembly.GetAssembly(typeof(RandomNumberResponse)));

    RandomNymberQuery request = new RandomNymberQuery
    {
        RequestTime = DateTime.Now
    };
    var response = await mediator.SendGeneric(request);

    //IServiceProvider services = new ServiceCollection();

    var apiresponse = JsonConvert.SerializeObject(response);

    Console.WriteLine(apiresponse);
}
else
{

    var mediator = new FlashCard.Shared.CQRS.Application.Logic.Mediator(Assembly.GetAssembly(typeof(RandomNumberResponse)));

    RandomNymberCommandRequest request = new RandomNymberCommandRequest
    {
        RequestTime = DateTime.Now
    };

    var response = await mediator.SendGeneric(request);

    //IServiceProvider services = new ServiceCollection();

    var apiresponse = JsonConvert.SerializeObject(response);

    Console.WriteLine(apiresponse);


}