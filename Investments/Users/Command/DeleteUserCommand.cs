using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command
{
    public class DeleteUserCommand : MediatR.IRequest<string>
    {
        public DeleteUserCommand()
        {
        }

        public DeleteUserCommand(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; set; }
    }
}
