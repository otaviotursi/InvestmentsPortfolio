using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Command;
using Infrastructure.Cache;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Users.Query.Handler
{
    public class GetByUserQueryHandler : IRequestHandler<GetByUserQuery, UserDomain>
    {
        private readonly IUserRepository _repository;
        private readonly ICacheHelper _cacheHelper;
        private readonly string keyCacheByQuery = "_user_by_{0}";

        public GetByUserQueryHandler(IUserRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<UserDomain> Handle(GetByUserQuery query, CancellationToken cancellationToken)
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
                var userCached = await _cacheHelper.GetDataAsync<UserDomain>(keyCache);
                if (userCached != null)
                {
                    return userCached;
                }

                // Buscar no repositório e salvar no cache
                var listUser = await _repository.GetBy(query.User, query.FullName, query.Id, cancellationToken);
                if (listUser != null)
                {
                    await _cacheHelper.SetDataAsync(keyCache, 10, listUser);
                }

                return listUser;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private string GenerateCacheKey(GetByUserQuery query)
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
