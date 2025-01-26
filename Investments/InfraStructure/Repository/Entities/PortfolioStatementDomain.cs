using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Infrastructure.Repository.Entities
{
    public class PortfolioStatementDomain
    {
        public Guid ProductId { get; set; }
        public ulong CustomerId { get; set; }
        public string ProductName { get; set; }
        public decimal AmountNegotiated { get; set; }
        public string OperationType { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
