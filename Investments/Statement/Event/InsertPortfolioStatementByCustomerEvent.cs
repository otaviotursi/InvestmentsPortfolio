using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Event
{
    public class InsertPortfolioStatementByCustomerEvent : PortfolioStatementDomain, INotification
    {

        public InsertPortfolioStatementByCustomerEvent()
        {

        }

        public InsertPortfolioStatementByCustomerEvent(Guid productId, ulong customerId, string productName, ulong amountNegotiated, string operationType)
        {
            ProductId = productId;
            CustomerId = customerId;
            ProductName = productName;
            AmountNegotiated = amountNegotiated;
            OperationType = operationType;
        }
    }
}
