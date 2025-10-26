using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medatortest.Command
{
    internal class RandomNumberCommandHandler : IRequestHandler<RandomNymberCommandRequest>
    {
        public async Task Handle(RandomNymberCommandRequest request)
        {
            Console.WriteLine($"Random number command executed at {request.RequestTime}: {new Random().Next(1, 100)}");
        }
    }
}
