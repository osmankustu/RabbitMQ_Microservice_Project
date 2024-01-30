using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Event.Events
{
    public class OrderCreatedIntegrationEvent :IntegrationEvent
    {
        public OrderCreatedIntegrationEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
