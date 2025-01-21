using Users.Command;
using Users.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Query;

namespace Investments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ulong? id, [FromQuery] string? user, [FromQuery] string? fullName, CancellationToken cancellationToken = default)
        {
            if(id == null && user == null && fullName == null)
            {
                var response = await _mediator.Send(new GetAllUserQuery(), cancellationToken);
                return Ok(response);

            } else
            {
                var response = await _mediator.Send(new GetByUserQuery(id, fullName, user), cancellationToken);
                return Ok(response);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken = default)
        {
            DeleteUserCommand command = new DeleteUserCommand();
            command.Id = id;
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
