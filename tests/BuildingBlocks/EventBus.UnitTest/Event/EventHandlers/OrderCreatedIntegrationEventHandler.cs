using EventBus.Base.Abstract;
using EventBus.UnitTest.Event.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Event.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationGenericEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Debug.WriteLine("on ack message :{message} Create Date : {createDate}", @event.Id, @event.CreateDate);
            return Task.CompletedTask;
        }
    }
}
