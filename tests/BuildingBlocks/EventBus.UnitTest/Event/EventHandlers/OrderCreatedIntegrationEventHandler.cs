using EventBus.Base.Abstract;
using EventBus.UnitTest.Event.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Event.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationGenericEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
