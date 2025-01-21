using Customers.Command;
using Customers.Repository.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Customers.Command;
using Customers.Repository.Interface;

namespace Investments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ulong? id, [FromQuery] string? user, [FromQuery] string? fullName, CancellationToken cancellationToken = default)
        {
            if(id == null && user == null && fullName == null)
            {
                var response = await _mediator.Send(new GetAllCustomerQuery(), cancellationToken);
                return Ok(response);

            } else
            {
                var response = await _mediator.Send(new GetByCustomerQuery(id, fullName, user), cancellationToken);
                return Ok(response);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken = default)
        {
            DeleteCustomerCommand command = new DeleteCustomerCommand();
            command.Id = id;
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
