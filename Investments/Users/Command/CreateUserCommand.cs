using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command
{
    public class CreateUserCommand : MediatR.IRequest<string>
    {
        public CreateUserCommand()
        {
        }

        public CreateUserCommand(string fullName, string user)
        {
            FullName = fullName;
            User = user;
        }

        public string FullName { get; set; }
        public string User { get; set; }
    }
}
