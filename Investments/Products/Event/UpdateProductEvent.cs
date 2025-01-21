using Infrastructure.Repository.Entities;
using MediatR;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Products.Event
{
    public class UpdateProductEvent : ProductDomain, INotification
    {

        public UpdateProductEvent()
        {

        }
        public UpdateProductEvent(ProductDomain product)
        {
            Id = product.Id;
            Name = product.Name;
            UnitPrice = product.UnitPrice;
            AvailableQuantity = product.AvailableQuantity;
            ProductType = product.ProductType;
            ExpirationDate = product.ExpirationDate;
        }
        public UpdateProductEvent(Guid productId, decimal availableQuantity, string operationType, decimal unitPrice, ulong userId, string productType,  string name)
        {
            Id = productId;
            AvailableQuantity = availableQuantity;
            UnitPrice = unitPrice;
            OperationType = operationType;
            UserId = userId;
            ProductType = productType;
            Name = name;
        }
        public string OperationType { get; set; }

    }
}
