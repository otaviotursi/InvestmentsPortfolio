using AutoMapper;
using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command.Handler
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IMediator mediator, IUserRepository repository, IMapper mapper)
        {
            _mediator = mediator;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var User = _mapper.Map<UserDomain>(command);
                await _repository.InsertAsync(User, cancellationToken);

                return await Task.FromResult("Cliente criado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
