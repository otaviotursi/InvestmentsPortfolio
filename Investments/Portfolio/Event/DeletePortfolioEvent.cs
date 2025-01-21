using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Portfolio.Event
{
    public class DeletePortfolioEvent : PortfolioRequest, INotification
    {

        public DeletePortfolioEvent()
        {

        }
        public DeletePortfolioEvent(PortfolioRequest portfolio)
        {
            CustomerId          = portfolio.CustomerId;
            ProductId           = portfolio.ProductId;
            ProductName         = portfolio.ProductName;
            AmountNegotiated    = portfolio.AmountNegotiated;
            ValueNegotiated     = portfolio.ValueNegotiated;
            OperationType       = portfolio.OperationType;
        }
    }
}
