using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository.Interface
{
    public interface IProductRepository
    {
        Task<List<ProductDomain>> GetAll(CancellationToken cancellationToken);
        Task<List<ProductDomain>> GetExpiritionByDateAll(int expirationDay, CancellationToken cancellationToken);
        Task InsertAsync(ProductDomain ProductDomain, CancellationToken cancellationToken);
        Task UpdateAsync(ProductDomain ProductDomain, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<ProductDomain> GetByName(string name, CancellationToken cancellationToken);
        Task<ProductDomain> GetById(Guid id, CancellationToken cancellationToken);
    }
}
