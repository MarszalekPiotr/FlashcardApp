

using FlashCard.Shared.CQRS.Application.Logic.Abstract;
namespace medatortest.Query
 
{
    internal class RandomNymberQuery:   IRequest<RandomNumberResponse> // RequestData
    {
        public DateTime RequestTime { get; set; }

    }
}