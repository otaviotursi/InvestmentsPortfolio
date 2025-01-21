using Customers.Repository.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command.Handler
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repositoryWrite;

        public DeleteCustomerCommandHandler(IMediator mediator, ICustomerRepository repositoryWrite)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
        }

        public async Task<string> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _repositoryWrite.DeleteAsync(command.Id, cancellationToken);

                return await Task.FromResult("Cliente excluido com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
