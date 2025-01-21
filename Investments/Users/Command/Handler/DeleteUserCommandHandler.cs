using Users.Repository.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command.Handler
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repositoryWrite;

        public DeleteUserCommandHandler(IMediator mediator, IUserRepository repositoryWrite)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
        }

        public async Task<string> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
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
