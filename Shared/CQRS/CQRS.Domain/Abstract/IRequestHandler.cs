using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard.Shared.CQRS.Domain.Abstract
{
    public interface IRequestHandler<TReq, TRes> 
    {
        public Task<TRes> Handle(TReq request);
    }
    
}
