using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository.Interface
{
    public interface IProductStatementRepository
    {
        
        Task InsertAsync(ProductDomain ProductDomain, CancellationToken cancellationToken);
        Task UpdateAsync(ProductDomain ProductDomain, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ProductDomain>> GetStatementBy(string? name, ulong? userId, DateTime? expirationDate, Guid? productId, CancellationToken cancellationToken);
    }
}
