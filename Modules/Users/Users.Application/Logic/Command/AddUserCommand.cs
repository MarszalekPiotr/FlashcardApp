using Flashcard.Modules.Users.Application.Interfaces.Repositories;
using Flashcard.Modules.Users.Application.Logic.Abstract;
using FlashCard.Shared.CQRS.Application.Logic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flashcard.Modules.Users.Application.Logic.Command
{
    public static  class AddUserCommand
    {

        public class Request : IRequest
        {
            public string Username { get; set; }  
            public string Email { get; set; } 
            public string Password { get; set; } 
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request>
        {   
            public Handler(IUserRepository userRepository) : base(userRepository)
            {
            }
            public async Task Handle(Request request)
            {   
                var user = new Domain.Entities.User
                {                  
                     FirstName = request.Username,
                     Email = request.Email,
                     PasswordHash = request.Password,
                     CreatedAt = DateTime.UtcNow,
                     Verified = false
                };
                await _userRepository.AddUserAsync(user);
            }
        }

    }
}
