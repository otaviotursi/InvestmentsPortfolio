using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Portfolio.Event;
using Portfolio.Repository.Interface;
using Products.Command;
using Products.Command.Handler;
using Products.Event;
using Statement.Event;

namespace Portfolio.Command.Handler
{
    public class OperatePortfolioCustomerCommandHandler : IRequestHandler<OperatePortfolioCustomerCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IPortfolioRepository _repository;
        private readonly IMapper _mapper;
        public OperatePortfolioCustomerCommandHandler(IMediator mediator, IPortfolioRepository repository, IMapper mapper)
        {
            _mediator = mediator;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<string> Handle(OperatePortfolioCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {

                var productByQuery = await _mediator.Send(new GetProductByQuery(command.ProductId), cancellationToken);
                var hasQuantity = productByQuery.AvailableQuantity >= command.AmountNegotiated;

                if (!hasQuantity && string.Equals(command.OperationType,"BUY", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Quantidade disponivel para compra insuficiente");
                }


                var portfolio = _mapper.Map<PortfolioRequest>(command);
                if (string.Equals(command.OperationType, "buy", StringComparison.OrdinalIgnoreCase)) {
                    portfolio.ValueNegotiated = command.AmountNegotiated * productByQuery.UnitPrice;
                    await _mediator.Publish(new InsertPortfolioEvent(portfolio), cancellationToken);

                    var availableQuantity = productByQuery.AvailableQuantity - command.AmountNegotiated;
                    await _mediator.Publish(new UpdateProductEvent(command.ProductId, availableQuantity, command.OperationType, productByQuery.UnitPrice, 0, productByQuery.ProductType, productByQuery.Name), cancellationToken);
                }
                else if (string.Equals(command.OperationType, "sell", StringComparison.OrdinalIgnoreCase))
                {
                    await _mediator.Publish(new DeletePortfolioEvent(portfolio), cancellationToken);

                    var availableQuantity = productByQuery.AvailableQuantity + command.AmountNegotiated;
                    await _mediator.Publish(new UpdateProductEvent(command.ProductId, availableQuantity, command.OperationType, productByQuery.UnitPrice, 0, productByQuery.ProductType, productByQuery.Name), cancellationToken);
                }

                //chama evento de statement portfolio
                var portfolioEvent= _mapper.Map<InsertPortfolioStatementByCustomerEvent>(command);
                await _mediator.Publish(portfolioEvent);

                return await Task.FromResult("trade realizado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
