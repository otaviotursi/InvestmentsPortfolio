using Infrastructure.Repository.Entities;
using MediatR;
using Portfolio.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Command
{
    public class GetPortfolioAllCustomersQuery : MediatR.IRequest<List<PortfolioDomain>>
    {
        public GetPortfolioAllCustomersQuery()
        {
        }
    }
}
