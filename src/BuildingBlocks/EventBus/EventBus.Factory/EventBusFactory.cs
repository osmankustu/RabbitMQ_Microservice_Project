﻿using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Factory
{
    public static class EventBusFactory 
    {
        public static IEventBus Create(EventBusConfig config,IServiceProvider serviceProvider)
        {
            return config.EventBusType switch
            {
                EventBusType.AzureServiceBus 
                =>  new EventBusAzure(config,serviceProvider) ,
                _ => new EventBusRabbitMQ(config, serviceProvider),
            };
        }
    }
}
