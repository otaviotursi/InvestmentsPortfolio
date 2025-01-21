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
    public class GetPortfolioAllCustomersQueryHandler : IRequestHandler<GetPortfolioAllCustomersQuery, List<PortfolioDomain>>
    {
        private readonly IMediator _mediator;
        private readonly IPortfolioRepository _repository;

        public GetPortfolioAllCustomersQueryHandler(IMediator mediator, IPortfolioRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<List<PortfolioDomain>> Handle(GetPortfolioAllCustomersQuery command, CancellationToken cancellationToken)
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
