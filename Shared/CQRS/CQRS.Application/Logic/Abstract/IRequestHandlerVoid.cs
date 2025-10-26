using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard.Shared.CQRS.Application.Logic.Abstract
{
    public interface IRequestHandler<TReq> 
    {
        public Task Handle(TReq request);
    }
}
