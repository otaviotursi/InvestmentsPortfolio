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

namespace Users.Query.Handler
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserDomain>>
    {
        private readonly IUserRepository _repository;
        private readonly string keyCacheAll = "_user_all";
        private readonly ICacheHelper _cacheHelper;

        public GetAllUserQueryHandler(IUserRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<UserDomain>> Handle(GetAllUserQuery Query, CancellationToken cancellationToken)
        {
            try
            {
                var userCached = await _cacheHelper.GetDataAsync<List<UserDomain>>(keyCacheAll);

                if (userCached != null && userCached.Count > 0)
                {
                    return userCached;
                }


                var listUser = await _repository.GetAll(cancellationToken);
                if (userCached != null && userCached.Count > 0)
                {
                    await _cacheHelper.SetDataAsync(keyCacheAll, 10, listUser);

                }
                return listUser;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
