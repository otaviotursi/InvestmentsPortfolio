using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Repository.Interface
{
    public interface IPortfolioRepository
    {
        Task<List<PortfolioDomain>> GetAll(CancellationToken cancellationToken);
        Task<PortfolioDomain> GetById(ulong customerId, CancellationToken cancellationToken);
        Task InsertAsync(PortfolioRequest product, CancellationToken cancellationToken);
        Task RemoveAsync(PortfolioRequest product, CancellationToken cancellationToken);
    }
}
