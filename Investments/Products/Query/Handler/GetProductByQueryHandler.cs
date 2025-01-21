using Infrastructure.Repository.Entities;
using MediatR;
using Products.Event;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command.Handler
{
    public class GetProductByQueryHandler : IRequestHandler<GetProductByQuery, ProductDomain>
    {
        private readonly IMediator _mediator;
        private readonly IProductRepository _repository;

        public GetProductByQueryHandler(IMediator mediator, IProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<ProductDomain> Handle(GetProductByQuery query, CancellationToken cancellationToken)
        {
            try
            {
                if(query.Name != null)
                {
                    return await _repository.GetByName(query.Name, cancellationToken);
                } else
                {
                    return await _repository.GetById(query.Id ?? Guid.Empty, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
