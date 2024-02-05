﻿using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class Buyer : BaseEntity, IAggregateRoute
    {
        public string Name { get; set; }

        private List<PaymentMethod> _paymentMethods;
        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

        protected Buyer()
        {
            _paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string name):this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeId,string alias,string cardNumber,
            string securityNumber,string cardHolderName,DateTime expiration,Guid orderId)
        {
            var existingPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

            if(existingPayment != null)
            {
                //raise Event //edilmek üzre
                AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));
                return existingPayment;
            }

            var payment = new PaymentMethod(alias,cardNumber,securityNumber,cardHolderName,expiration,cardTypeId);

            _paymentMethods.Add(payment);

            //raise Event //edilme üzere 
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this,payment, orderId));

            return payment;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) || 
                    (obj is Buyer buyer &&
                    Id.Equals(buyer.Id) && 
                    Name == buyer.Name) ;
        }
    }
}
