using MediatR;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Events
{
    public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
    {
        public Buyer Buyer { get; set; }
        public PaymentMethod Payment { get; set; }
        public Guid OrderId { get; set; }

        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment,Guid orderId)
        {
            Buyer = buyer;
            Payment = payment;
            OrderId = orderId;
        }

    }
}
