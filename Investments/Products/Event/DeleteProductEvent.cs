using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Event
{
    public class DeleteProductEvent : INotification
    {
        public Guid Id { get; set; }
        public DeleteProductEvent(Guid id)
        {
            Id = id;
        }
        public DeleteProductEvent()
        {
        }

    }
}
