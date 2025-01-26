using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Statement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Command.Handler
{
    public class GetPortfolioStatementByCustomerQueryHandler : IRequestHandler<GetPortfolioStatementByCustomerQuery, List<PortfolioStatementDomain>>
    {
        private readonly IPortfolioStatementRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string _keyCacheByCustomer = "_portfolio_statement_by_{0}";

        public GetPortfolioStatementByCustomerQueryHandler(IPortfolioStatementRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<PortfolioStatementDomain>> Handle(GetPortfolioStatementByCustomerQuery command, CancellationToken cancellationToken)
        {
            try
            {
                // Gera a chave do cache
                string keyCache = GenerateCacheKey(command);

                // Se não houver parâmetros válidos, retorna direto sem salvar no cache
                if (keyCache == null)
                {
                    return await _repository.GetByCustomerId(command.CustomerId, cancellationToken);
                }

                // Tentar obter do cache
                var portfolioCached = await _cacheHelper.GetDataAsync<List<PortfolioStatementDomain>>(keyCache);
                if (portfolioCached != null)
                {
                    return portfolioCached;
                }

                // Buscar no repositório e salvar no cache
                var portfolioList = await _repository.GetByCustomerId(command.CustomerId, cancellationToken);
                if (portfolioList != null)
                {
                    await _cacheHelper.SetDataAsync(keyCache, 10, portfolioList);
                }

                return portfolioList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GenerateCacheKey(GetPortfolioStatementByCustomerQuery command)
        {
            // Validação do CustomerId
            bool isCustomerIdInvalid = command.CustomerId <= 0;

            // Se CustomerId for inválido, retorna null
            if (isCustomerIdInvalid)
            {
                return null;
            }

            // Montar a chave do cache com CustomerId
            return string.Format(_keyCacheByCustomer, $"CustomerId:{command.CustomerId}");
        }
    }

}
