using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Query
{
    public class GetStatementByProductQuery : MediatR.IRequest<List<ProductDomain>>
    {
        public GetStatementByProductQuery()
        {
        }
        public GetStatementByProductQuery(string? productName, ulong? userId, DateTime? expirationDate, Guid? productId)
        {
            Name = productName;
            UserId = userId;
            ProductId = productId;
            ExpirationDate = expirationDate;
        }

        public string? Name { get; set; }
        public ulong? UserId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Guid? ProductId { get; set; }
        

    }
}
