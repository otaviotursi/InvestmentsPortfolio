using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Newtonsoft.Json;
using Portfolio.Repository.Interface;

namespace Portfolio.Command.Handler
{
    public class GetPortfolioAllCustomersQueryHandler : IRequestHandler<GetPortfolioAllCustomersQuery, List<PortfolioDomain>>
    {
        private readonly IPortfolioRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string keyCacheAll = "portfolio_all";

        public GetPortfolioAllCustomersQueryHandler(IPortfolioRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<PortfolioDomain>> Handle(GetPortfolioAllCustomersQuery command, CancellationToken cancellationToken)
        {
            try
            {
                var portfolioCached = await _cacheHelper.GetDataAsync<List<PortfolioDomain>>(keyCacheAll);

                if (portfolioCached != null && portfolioCached.Count > 0)
                {
                    return portfolioCached;
                }

                var listPortfolio = await _repository.GetAll(cancellationToken);
                await _cacheHelper.SetDataAsync(keyCacheAll, 10, JsonConvert.SerializeObject(listPortfolio));
                return listPortfolio;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
