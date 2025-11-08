using Flashcard.Modules.Users.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcard.Modules.Users.Application.Logic.Abstract
{
    public abstract class BaseCommandHandler
    {   
        public IUserRepository _userRepository;
        public BaseCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
