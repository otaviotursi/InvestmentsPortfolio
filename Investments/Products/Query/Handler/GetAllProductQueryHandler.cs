using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Newtonsoft.Json;
using Products.Repository.Interface;


namespace Products.Command.Handler
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<ProductDomain>>
    {
        private readonly IProductRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string keyCacheAll = "_product_all";


        public GetAllProductQueryHandler(IProductRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<ProductDomain>> Handle(GetAllProductQuery command, CancellationToken cancellationToken)
        {
            try
            {
                var productCached = await _cacheHelper.GetDataAsync<List<ProductDomain>>(keyCacheAll);

                if (productCached != null && productCached.Count > 0)
                {
                    return productCached;
                }

                var listProduct = await _repository.GetAll(cancellationToken);
                if (productCached != null && productCached.Count > 0){
                    await _cacheHelper.SetDataAsync(keyCacheAll, 10, listProduct);

                }

                return listProduct;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
