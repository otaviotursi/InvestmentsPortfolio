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
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<ProductDomain>>
    {
        private readonly IMediator _mediator;
        private readonly IProductRepository _repository;

        public GetAllProductQueryHandler(IMediator mediator, IProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<List<ProductDomain>> Handle(GetAllProductQuery command, CancellationToken cancellationToken)
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
