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
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repositoryWrite;
        private readonly IMapper _mapper;

        public UpdateCustomerCommandHandler(IMediator mediator, ICustomerRepository repositoryWrite, IMapper mapper)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var customer = _mapper.Map<CustomerDomain>(command);
                await _repositoryWrite.UpdateAsync(customer, cancellationToken);

                return string.Format("Cliente {0} alterado com sucesso", command.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
