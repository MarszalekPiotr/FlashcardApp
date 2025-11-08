using Flashcard.Modules.Users.Application.Logic.Command;
using FlashCard.Shared.CQRS.Application.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlashCard.Modules.Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {    
        private Mediator _mediator;
        public UserController(Mediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async  Task<IActionResult> CreateUser(AddUserCommand.Request request)
        {
            await _mediator.SendCommand(request);
            return Ok("User created");
        }
    }
}
