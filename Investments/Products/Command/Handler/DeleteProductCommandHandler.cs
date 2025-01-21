using MediatR;
using Products.Event;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command.Handler
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, string>
    {
        private readonly IMediator _mediator;

        public DeleteProductCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<string> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            try
            {

                await _mediator.Publish(new DeleteProductEvent(command.Id));

                return await Task.FromResult("Produto excluido com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
