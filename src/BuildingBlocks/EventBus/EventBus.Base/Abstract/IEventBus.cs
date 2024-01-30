﻿using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstract
{
    public interface IEventBus :IDisposable
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationGenericEventHandler<T>;
        void Unsubscribe<T,TH>() where T : IntegrationEvent where TH : IIntegrationGenericEventHandler<T>;
    }
}
