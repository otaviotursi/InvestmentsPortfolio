using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Command;

namespace Users.Query.Handler
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserDomain>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repository;

        public GetAllUserQueryHandler(IMediator mediator, IUserRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<List<UserDomain>> Handle(GetAllUserQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetAll(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
