using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Products.Event;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Query.Handler
{
    public class GetStatementByProductQueryHandler : IRequestHandler<GetStatementByProductQuery, List<ProductDomain>>
    {
        private readonly IProductStatementRepository _repository;
        private readonly string _keyCacheByName = "_product_statement_by_{0}";
        private readonly ICacheHelper _cacheHelper;

        public GetStatementByProductQueryHandler(IProductStatementRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<ProductDomain>> Handle(GetStatementByProductQuery command, CancellationToken cancellationToken)
        {
            try
            {
                // Gera a chave do cache
                string keyCache = GenerateCacheKey(command);

                // Se não houver parâmetros válidos, retorna direto sem salvar no cache
                if (keyCache == null)
                {
                    return await _repository.GetStatementBy(command.Name, command.UserId, command.ExpirationDate, command.ProductId, cancellationToken);
                }

                // Tentar obter do cache
                var productCached = await _cacheHelper.GetDataAsync<List<ProductDomain>>(keyCache);
                if (productCached != null)
                {
                    return productCached;
                }

                // Buscar no repositório e salvar no cache
                var productList = await _repository.GetStatementBy(command.Name, command.UserId, command.ExpirationDate, command.ProductId, cancellationToken);
                if (productList != null)
                {
                    await _cacheHelper.SetDataAsync(keyCache, 10, productList);
                }

                return productList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GenerateCacheKey(GetStatementByProductQuery command)
        {
            // Validação dos parâmetros
            bool isNameNull = string.IsNullOrEmpty(command.Name);
            bool isUserIdInvalid = command.UserId <= 0;
            bool isExpirationDateNull = command.ExpirationDate == null;
            bool isProductIdInvalid = command.ProductId == Guid.Empty;

            // Se todos forem inválidos, retorna null
            if (isNameNull && isUserIdInvalid && isExpirationDateNull && isProductIdInvalid)
            {
                return null;
            }

            // Montar partes da chave do cache
            var keyParts = new List<string>();
            if (!isNameNull) keyParts.Add($"Name:{command.Name}");
            if (!isUserIdInvalid) keyParts.Add($"UserId:{command.UserId}");
            if (!isExpirationDateNull) keyParts.Add($"ExpirationDate:{command.ExpirationDate:yyyy-MM-dd}");
            if (!isProductIdInvalid) keyParts.Add($"ProductId:{command.ProductId}");

            // Combinar a chave do cache com _keyCacheByName
            return string.Format(_keyCacheByName, string.Join("_", keyParts));
        }
    }
}
