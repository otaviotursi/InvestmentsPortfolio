using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Command
{
    public class GetPortfolioByCustomerQuery : MediatR.IRequest<PortfolioDomain>
    {
        public GetPortfolioByCustomerQuery()
        {
        }
        public GetPortfolioByCustomerQuery(ulong customerId)
        {
            CustomerId = customerId;
        }

        public ulong CustomerId { get; set; }


    }
}
