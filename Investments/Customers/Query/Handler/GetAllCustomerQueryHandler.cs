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
    public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, List<CustomerDomain>>
    {
        private readonly ICustomerRepository _repository;
        private readonly string keyCacheAll = "_customer_all";
        private readonly ICacheHelper _cacheHelper;

        public GetAllCustomerQueryHandler(ICustomerRepository repository, ICacheHelper cacheHelper)
        {
            _repository = repository;
            _cacheHelper = cacheHelper;
        }

        public async Task<List<CustomerDomain>> Handle(GetAllCustomerQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var customerCached = await _cacheHelper.GetDataAsync<List<CustomerDomain>>(keyCacheAll);

                if (customerCached != null && customerCached.Count > 0)
                {
                    return customerCached;
                }

                var listCustomer = await _repository.GetAll(cancellationToken);
                if (customerCached != null && customerCached.Count > 0)
                {
                    await _cacheHelper.SetDataAsync(keyCacheAll, 10, listCustomer);

                }
                return listCustomer;

            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
