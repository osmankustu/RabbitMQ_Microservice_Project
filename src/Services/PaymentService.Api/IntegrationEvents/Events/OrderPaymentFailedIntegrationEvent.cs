using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string Message { get; }
        public OrderPaymentFailedIntegrationEvent(int orderId,string message)
        {
            OrderId = orderId;
            Message = message;
        } 
    }
}
