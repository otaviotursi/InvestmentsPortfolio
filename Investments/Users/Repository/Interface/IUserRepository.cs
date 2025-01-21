using Users.Command;
using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Repository.Interface
{
    public interface IUserRepository
    {
        Task DeleteAsync(ulong id, CancellationToken cancellationToken);
        Task<List<UserDomain>> GetAll(CancellationToken cancellationToken);
        Task<UserDomain> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken);
        Task InsertAsync(UserDomain User, CancellationToken cancellationToken);
        Task UpdateAsync(UserDomain User, CancellationToken cancellationToken);
    }
}
