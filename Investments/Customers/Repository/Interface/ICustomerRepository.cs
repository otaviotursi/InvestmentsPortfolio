using Customers.Command;
using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task DeleteAsync(ulong id, CancellationToken cancellationToken);
        Task<List<CustomerDomain>> GetAll(CancellationToken cancellationToken);
        Task<CustomerDomain> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken);
        Task InsertAsync(CustomerDomain customer, CancellationToken cancellationToken);
        Task UpdateAsync(CustomerDomain customer, CancellationToken cancellationToken);
    }
}
