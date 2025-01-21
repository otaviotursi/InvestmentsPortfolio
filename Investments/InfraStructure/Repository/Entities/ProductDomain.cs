using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Entities
{
    public class ProductDomain
    {
        public ProductDomain(Guid id, string name, decimal unitPrice, decimal availableQuantity, string productType, DateTime expirationDate, ulong userId)
        {
            Id = id;
            Name = name;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ProductType = productType;
            ExpirationDate = expirationDate;
            UserId = userId;
        }

        public ProductDomain() { }

        public ProductDomain(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ProductType { get; set; }
        public decimal AvailableQuantity { get; set; }
        public ulong UserId { get; set; }
        public string? Type { get; set; }
    }
}
