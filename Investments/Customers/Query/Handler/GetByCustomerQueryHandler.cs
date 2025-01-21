using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command.Handler
{
    public class GetByCustomerQueryHandler : IRequestHandler<GetByCustomerQuery, CustomerDomain>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repository;

        public GetByCustomerQueryHandler(IMediator mediator, ICustomerRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<CustomerDomain> Handle(GetByCustomerQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetBy(command.User, command.FullName, command.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
