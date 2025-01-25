using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Newtonsoft.Json;
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
        private readonly IPortfolioRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string _keyCacheByName = "portfolio_name_{0}";
        private readonly string keyCacheById = "portfolio_id_{0}";

        public GetPortfolioByCustomerQueryHandler(IPortfolioRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<PortfolioDomain> Handle(GetPortfolioByCustomerQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await GetById(command, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }


        private async Task<PortfolioDomain> GetById(GetPortfolioByCustomerQuery query, CancellationToken cancellationToken)
        {
            string keyCache = string.Format(keyCacheById, query.CustomerId);
            var portfolioCached = await _cacheHelper.GetDataAsync<PortfolioDomain>(keyCache);
            if (portfolioCached != null)
            {
                return portfolioCached;
            }
            var portfolio = await _repository.GetById(query.CustomerId, cancellationToken);
            await _cacheHelper.SetDataAsync(keyCache, 10, JsonConvert.SerializeObject(portfolio));
            return portfolio;
        }
    }
}
