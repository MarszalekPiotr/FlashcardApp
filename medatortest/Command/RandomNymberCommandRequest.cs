using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medatortest.Command
{
    internal class RandomNymberCommandRequest : IRequest
    {
        public DateTime RequestTime { get; set; }
    }
}
