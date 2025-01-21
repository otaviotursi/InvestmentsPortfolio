using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command
{
    public class CreateCustomerCommand : MediatR.IRequest<string>
    {
        public CreateCustomerCommand()
        {
        }

        public CreateCustomerCommand(string fullName, string user)
        {
            FullName = fullName;
            User = user;
        }

        public string FullName { get; set; }
        public string User { get; set; }
    }
}
