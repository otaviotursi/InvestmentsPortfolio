using Customers.Repository.Interface;
using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Query.Handler
{
    public class GetByCustomerQueryHandler : IRequestHandler<GetByCustomerQuery, CustomerDomain>
    {
        private readonly ICustomerRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string keyCacheByQuery = "_customer_by_{0}";

        public GetByCustomerQueryHandler(ICustomerRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }
        public async Task<CustomerDomain> Handle(GetByCustomerQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Gera a chave do cache
                string keyCache = GenerateCacheKey(query);

                // Se não houver parâmetros válidos, retorna direto sem salvar no cache
                if (keyCache == null)
                {
                    return await _repository.GetBy(query.User, query.FullName, query.Id, cancellationToken);
                }

                // Tentar obter do cache
                var customerCached = await _cacheHelper.GetDataAsync<CustomerDomain>(keyCache);
                if (customerCached != null)
                {
                    return customerCached;
                }

                // Buscar no repositório e salvar no cache
                var customer = await _repository.GetBy(query.User, query.FullName, query.Id, cancellationToken);
                if (customer != null)
                {
                    await _cacheHelper.SetDataAsync(keyCache, 10, customer);
                }

                return customer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    private string GenerateCacheKey(GetByCustomerQuery query)
        {
            // Validação dos parâmetros
            bool isUserNull = string.IsNullOrEmpty(query.User);
            bool isFullNameNull = string.IsNullOrEmpty(query.FullName);
            bool isIdInvalid = query.Id <= 0;

            // Se todos forem inválidos, retorna null
            if (isUserNull && isFullNameNull && isIdInvalid)
            {
                return null;
            }

            // Montar partes da chave do cache
            var keyParts = new List<string>();
            if (!isUserNull) keyParts.Add($"User:{query.User}");
            if (!isFullNameNull) keyParts.Add($"FullName:{query.FullName}");
            if (!isIdInvalid) keyParts.Add($"Id:{query.Id}");

            // Combinar a chave do cache com keyCacheByQuery
            return string.Format(keyCacheByQuery, string.Join("_", keyParts));
        }
    }
}
