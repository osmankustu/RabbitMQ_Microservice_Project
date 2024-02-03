using EventBus.Base.Abstract;
using Microsoft.Extensions.Logging;
using NotificationService.ConsoleApp.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.ConsoleApp.IntegrationEvents.EventHandlers
{
    public class OrderPaymentFailedIntegrationEventHandler : IIntegrationGenericEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            //send Fail Email or Notification (sms,push)
            _logger.LogInformation($"Order Payment failed  with OrderId : {@event.OrderId} ,ErrorMessage : {@event.Message}");

            return Task.CompletedTask;
        }
    }
}
