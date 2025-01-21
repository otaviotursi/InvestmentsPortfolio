using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetProductByQuery : MediatR.IRequest<ProductDomain>
    {
        public GetProductByQuery()
        {
        }
        public GetProductByQuery(string name)
        {
            Name = name;
        }
        public GetProductByQuery(Guid id)
        {
            Id = id;
        }

        public string? Name { get; set; }
        public Guid? Id { get; set; }


    }
}
