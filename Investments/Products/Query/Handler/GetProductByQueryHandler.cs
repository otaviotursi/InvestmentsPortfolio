using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Newtonsoft.Json;
using Products.Repository.Interface;
namespace Products.Command.Handler
{
    public class GetProductByQueryHandler : IRequestHandler<GetProductByQuery, ProductDomain>
    {
        private readonly IProductRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string _keyCacheByName = "product_name_{0}";
        private readonly string keyCacheById = "product_id_{0}";

        public GetProductByQueryHandler(IProductRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<ProductDomain> Handle(GetProductByQuery query, CancellationToken cancellationToken)
        {
            try
            {
                if(query.Name != null)
                {
                    return await GetByName(query, cancellationToken);
                }
                else
                {
                    return await GetById(query, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private async Task<ProductDomain> GetByName(GetProductByQuery query, CancellationToken cancellationToken)
        {
            string keyCache = string.Format(_keyCacheByName, query.Name);
            var productCached = await _cacheHelper.GetDataAsync<ProductDomain>(keyCache);

            if (productCached != null)
            {
                return productCached;
            }
            var product = await _repository.GetByName(query.Name, cancellationToken);
            await _cacheHelper.SetDataAsync(keyCache, 10, JsonConvert.SerializeObject(product));
            return product;
        }

        private async Task<ProductDomain> GetById(GetProductByQuery query, CancellationToken cancellationToken)
        {
            string keyCache = string.Format(keyCacheById, query.Id);
            var productCached = await _cacheHelper.GetDataAsync<ProductDomain>(keyCache);
            if (productCached != null)
            {
                return productCached;
            }
            var product = await _repository.GetById((Guid)query.Id, cancellationToken);
            await _cacheHelper.SetDataAsync(keyCache, 10, JsonConvert.SerializeObject(product));
            return product;
        }
    }
}
