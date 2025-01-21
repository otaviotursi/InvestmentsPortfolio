using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Command
{
    public class GetPortfolioStatementByCustomerQuery : MediatR.IRequest<List<PortfolioStatementDomain>>
    {
        public GetPortfolioStatementByCustomerQuery()
        {
        }
        public GetPortfolioStatementByCustomerQuery(ulong customerId)
        {
            CustomerId = customerId;
        }

        public ulong CustomerId { get; set; }

    }
}
