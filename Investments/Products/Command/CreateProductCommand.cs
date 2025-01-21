using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class CreateProductCommand : MediatR.IRequest<string>
    {
        public CreateProductCommand()
        {
        }

        public CreateProductCommand(Guid id, string name, string productType, decimal unitPrice, decimal availableQuantity, DateTime expirationDate, ulong userId)
        {
            Id = id;
            Name = name;
            ProductType = productType;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ExpirationDate = expirationDate;
            UserId = userId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string ProductType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AvailableQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ulong UserId { get; set; }
    }
}
