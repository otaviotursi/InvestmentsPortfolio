using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command
{
    public class DeleteCustomerCommand : MediatR.IRequest<string>
    {
        public DeleteCustomerCommand()
        {
        }

        public DeleteCustomerCommand(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; set; }
    }
}
