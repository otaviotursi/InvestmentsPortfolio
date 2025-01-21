using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command
{
    public class UpdateCustomerCommand : MediatR.IRequest<string>
    {
        public UpdateCustomerCommand()
        {
        }

        public UpdateCustomerCommand(ulong id, string fullName, string user)
        {
            Id = id;
            FullName = fullName;
            User = user;
        }
        public ulong Id { get; set; }
        public string FullName { get; set; }
        public string User { get; set; }

    }
}
