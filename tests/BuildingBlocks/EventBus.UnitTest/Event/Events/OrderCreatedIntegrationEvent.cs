﻿using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Event.Events
{
    public class OrderCreatedIntegrationEvent :IntegrationEvent
    {
        public OrderCreatedIntegrationEvent(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
