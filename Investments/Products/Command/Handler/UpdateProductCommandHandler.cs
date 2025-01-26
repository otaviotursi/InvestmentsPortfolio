using AutoMapper;
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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public Task<string> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {

                var productEvent = _mapper.Map<UpdateProductEvent>(command);
                _mediator.Publish(productEvent);

                return Task.FromResult("Produto alterado com sucesso");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
