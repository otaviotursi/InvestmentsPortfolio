using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Query
{
    public class GetAllCustomerQuery : MediatR.IRequest<List<CustomerDomain>>
    {
    }
}
