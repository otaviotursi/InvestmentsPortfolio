using MediatR;
using Microsoft.AspNetCore.Mvc;
using Products.Command;
using Products.Query;
using Products.Repository.Interface;

namespace Investments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IProductRepository _productReadRepository;

        public ProductController(IMediator mediator, IProductRepository productReadRepository)
        {
            _mediator = mediator;
            _productReadRepository = productReadRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? productName, [FromQuery] Guid? productId, CancellationToken cancellationToken = default)
        {
            if(productName == null && productId == null)
            {
                var response = await _mediator.Send(new GetAllProductQuery(), cancellationToken);
                return Ok(response);

            } else if (productId != null)
            {
                var response = await _mediator.Send(new GetProductByQuery(productId??Guid.Empty), cancellationToken);
                return Ok(response);
            }
            else
            {
                var response = await _mediator.Send(new GetProductByQuery(productName), cancellationToken);
                return Ok(response);
            }

        }
        [HttpGet("statement")]
        public async Task<IActionResult> GetStatementByName([FromQuery] ulong? userId, [FromQuery] string? productName, [FromQuery] DateTime? expirationDate, [FromQuery] Guid? productId, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetStatementByProductQuery(productName, userId, expirationDate, productId), cancellationToken);

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            DeleteProductCommand command = new DeleteProductCommand();
            command.Id = id;
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
