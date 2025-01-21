using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Entities
{
    public class PortfolioDomain
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ulong CustomerId { get; set; }
        public List<ItemPortfolio> ItensPortfolio { get; set; }
    }
    public class ItemPortfolio
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal AmountNegotiated { get; set; }
        public decimal ValueNegotiated { get; set; }
    }

    public class PortfolioRequest
    {
        public ulong CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal AmountNegotiated { get; set; }
        public decimal? ValueNegotiated { get; set; }
        public string OperationType { get; set; }
    }
}
