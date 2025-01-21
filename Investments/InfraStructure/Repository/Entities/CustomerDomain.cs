using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Entities
{
    public class CustomerDomain
    {
        public CustomerDomain(ulong id, string fullName, string user)
        {
            Id = id;
            FullName = fullName;
            User = user;
        }

        public CustomerDomain() { }

        public CustomerDomain(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; set; }
        public string FullName { get; set; }
        public string User { get; set; }
        public DateTime CreateUserDate { get; set; } = DateTime.Now;
    }
}
