using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class Order : BaseEntity, IAggregateRoute
    {
        public DateTime OrderDate { get; private set; }
        public int Quantity { get; private set; }
        public string Description { get; private set; }
        public Guid? BuyerId { get; private set; }
        public Buyer Buyer { get; private set; }
        public Address Address { get; private set; }
        private int orderStatusId;
        public OrderStatus OrderStatus { get; private set; }
        
        
        private readonly List<Orderitem> _orderItems;
        public IReadOnlyCollection<Orderitem> OrderItems => _orderItems;
        public Guid? PaymentMethodId { get; set; }
       
        protected Order()
        {
            Id = Guid.NewGuid();
            _orderItems = new List<Orderitem>();
        }

        public Order(string userName, Address address, int cardTypeId,string cardNumber,string cardSecurityNumber,
            string cardHolderName,DateTime cardExpiration,Guid? paymentMethodId, Guid? buyerId = null) :this()
        {
            BuyerId = buyerId;
            orderStatusId = OrderStatus.Submitted.Id;
            OrderDate = DateTime.UtcNow;
            Address = address;
            PaymentMethodId = paymentMethodId;

            AddOrderStaredDomainEvent(userName,cardTypeId,cardNumber,
                                       cardSecurityNumber,cardHolderName,cardExpiration);
        }

        private void AddOrderStaredDomainEvent(string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this,userName,cardTypeId,
                                                                     cardNumber, cardSecurityNumber,
                                                                     cardHolderName,cardExpiration);

            this.AddDomainEvent(orderStartedDomainEvent);
        }

        public void AddOrderItem(int productId,string prodductName,decimal unitPrice,int units = 1)
        {
            var orderItem = new Orderitem(productId, prodductName,unitPrice,units);
            _orderItems.Add(orderItem);
        }

        public void SetBuyerId(Guid buyerId)
        {
            BuyerId = BuyerId;
        }

        public void SetPaymentMethodId(Guid paymentMethodId)
        {
            PaymentMethodId = PaymentMethodId;
        }
    }
}
