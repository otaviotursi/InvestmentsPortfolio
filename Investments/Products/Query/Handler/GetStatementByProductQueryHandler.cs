using Infrastructure.Repository.Entities;
using MediatR;
using Products.Event;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Query.Handler
{
    public class GetStatementByProductQueryHandler : IRequestHandler<GetStatementByProductQuery, List<ProductDomain>>
    {
        private readonly IMediator _mediator;
        private readonly IProductStatementRepository _repository;

        public GetStatementByProductQueryHandler(IMediator mediator, IProductStatementRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<List<ProductDomain>> Handle(GetStatementByProductQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetStatementBy(command.Name, command.UserId, command.ExpirationDate,command.ProductId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
