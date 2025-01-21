using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Query
{
    public class GetByUserQuery : MediatR.IRequest<UserDomain>
    {
        public GetByUserQuery()
        {
        }
        public GetByUserQuery(ulong? id, string? fullName, string? user)
        {
            Id = id;
            FullName = fullName;
            User = user;
        }
        public ulong? Id { get; set; }
        public string? FullName { get; set; }
        public string? User { get; set; }

    }
}
