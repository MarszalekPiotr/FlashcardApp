using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medatortest.Query
{
    internal class RandomNymberQueryHandler : IRequestHandler<RandomNymberQuery, RandomNumberResponse>
    {
        public async Task<RandomNumberResponse> Handle(RandomNymberQuery request)
        {
            return new RandomNumberResponse
            {
                resposne = $"Random number generated at {request.RequestTime}: {new Random().Next(1, 100)}"
            };
        }
    }
}
