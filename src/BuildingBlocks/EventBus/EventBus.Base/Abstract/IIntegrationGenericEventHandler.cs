using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstract
{
    public interface IIntegrationGenericEventHandler<TIntegrationEvent> : IIntegrationGenericEventHandler where TIntegrationEvent :IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
