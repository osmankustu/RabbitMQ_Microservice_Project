using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using EventBus.UnitTest.Event.EventHandlers;
using EventBus.UnitTest.Event.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection services;
        public EventBusTests()
        {
            services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
        }

        [TestMethod]
        public void TestMethod1()
        {
            

            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "Osmankustu",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "192.168.1.39",
                        UserName = "root",
                        Password = "root",
                        Port = 5672,
                        AuthMechanisms = new List<IAuthMechanismFactory>() { default},
                        TopologyRecoveryExceptionHandler = default,
                        Uri= new Uri("amqp://root:root@192.168.1.39:5672"),
                        

                    }
                };
                return EventBusFactory.Create(config, sp);
            });

           


            var service = services.BuildServiceProvider();
            var eventBus = service.GetRequiredService<IEventBus>();


            eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
            var i = 0;
            while (i<100)
            {
               var x = new OrderCreatedIntegrationEvent(Guid.NewGuid());
                eventBus.Publish(x);
                i++;
            }
            eventBus.Unsubscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
        }
    }
}