using Infrastructure.Repository.Entities;
using MediatR;
using Portfolio.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Command.Handler
{
    public class GetPortfolioByCustomerQueryHandler : IRequestHandler<GetPortfolioByCustomerQuery, PortfolioDomain>
    {
        private readonly IMediator _mediator;
        private readonly IPortfolioRepository _repository;

        public GetPortfolioByCustomerQueryHandler(IMediator mediator, IPortfolioRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<PortfolioDomain> Handle(GetPortfolioByCustomerQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetByName(command.CustomerId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
