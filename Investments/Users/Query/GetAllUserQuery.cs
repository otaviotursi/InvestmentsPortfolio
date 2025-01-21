using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Query
{
    public class GetAllUserQuery : MediatR.IRequest<List<UserDomain>>
    {
    }
}
