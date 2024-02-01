using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using EventBus.UnitTest.Event.EventHandlers;
using EventBus.UnitTest.Event.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using static System.Net.Mime.MediaTypeNames;

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
        public void subscribe_event_on_rabbitmq_test()
        {
            //If you want to consume, remove the unsubscribe method
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var service = services.BuildServiceProvider();
            var eventBus = service.GetRequiredService<IEventBus>();


           eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
           eventBus.Unsubscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
        }

        [TestMethod]
        public void send_message_to_rabbitmq_test()
        {
            //If you want to consume, remove the unsubscribe method
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var sp = services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();

            var q = new List<OrderCreatedIntegrationEvent>() {
                new OrderCreatedIntegrationEvent(1),
                new OrderCreatedIntegrationEvent(2),
                new OrderCreatedIntegrationEvent(3),
                new OrderCreatedIntegrationEvent(5)

            };
            foreach (var item in q)
            {
                eventBus.Publish(item);
            }
        }


        //Test Passed
        //[TestMethod]
        //public void subscribe_event_on_AzureServiceBus_test()
        //{
        //    //If you want to consume, remove the unsubscribe method
        //    services.AddSingleton<IEventBus>(sp =>
        //    {
        //        return EventBusFactory.Create(GetAzureServiceBusConfig(), sp);
        //    });

        //    var service = services.BuildServiceProvider();
        //    var eventBus = service.GetRequiredService<IEventBus>();

        //    // service bus basic tier is not supported topics 

        //    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
           
        //    OrderCreatedIntegrationEvent x = new OrderCreatedIntegrationEvent(1);
        //    //eventBus.Unsubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        //}


        //Test Passed
        //[TestMethod]
        //public void send_message_to_azureServiceBus_test()
        //{
        //    services.AddSingleton<IEventBus>(sp =>
        //    {
        //        return EventBusFactory.Create(GetAzureServiceBusConfig(), sp);
        //    });

        //    var sp = services.BuildServiceProvider();
        //    var eventBus = sp.GetRequiredService<IEventBus>();

        //    eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        //}

        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
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
                    AuthMechanisms = new List<IAuthMechanismFactory>() { default },
                    TopologyRecoveryExceptionHandler = default,
                    Uri = new Uri("amqp://root:root@192.168.1.39:5672"),
                }

            };
        }

        //private EventBusConfig GetAzureServiceBusConfig()
        //{
        //    return new EventBusConfig()
        //    {
        //        ConnectionRetryCount = 5,
        //        SubscriberClientAppName = "EventBus.UnitTest",
        //        DefaultTopicName = "osmankustu",
        //        EventBusType = EventBusType.AzureServiceBus,
        //        EventNameSuffix = "IntegrationEvent",
        //        EventBusConnectionString = "Endpoint=sb://osmankustu.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Iu1sXsAIoVfGS68o0PZ+qaWHiDCEjwak7+ASbJ4M67M="
        //    };
        //}

    }
}