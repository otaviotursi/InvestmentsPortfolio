using AutoMapper;
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
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CreateCustomerCommandHandler(IMediator mediator, ICustomerRepository repository, IMapper mapper)
        {
            _mediator = mediator;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var customer = _mapper.Map<CustomerDomain>(command);
                await _repository.InsertAsync(customer, cancellationToken);

                return await Task.FromResult("Cliente criado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
