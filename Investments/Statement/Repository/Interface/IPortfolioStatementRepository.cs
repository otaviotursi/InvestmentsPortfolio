using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Repository.Interface
{
    public interface IPortfolioStatementRepository
    {
        Task<List<PortfolioStatementDomain>> GetByCustomerId(ulong customerId, CancellationToken cancellationToken);
        Task InsertAsync(PortfolioStatementDomain? sellProduct, CancellationToken stoppingToken);
    }
}
