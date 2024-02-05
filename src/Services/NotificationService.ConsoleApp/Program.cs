using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.ConsoleApp.IntegrationEvents.EventHandlers;
using NotificationService.ConsoleApp.IntegrationEvents.Events;
using RabbitMQ.Client;


ServiceCollection service = new ServiceCollection();
ConfigureService(service);  
var sp = service.BuildServiceProvider();
IEventBus eventBus= sp.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent,OrderPaymentFailedIntegrationEventHandler>();

Console.WriteLine("Notification service app is running...");

Console.ReadLine();

 static void ConfigureService(IServiceCollection services)
{
  

    services.AddLogging(configure =>
    {
        configure.AddConsole();
    });
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();
    services.AddSingleton<IEventBus>(sp =>
    {
        var conf = new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventBusType = EventBusType.RabbitMQ,
            Connection = new ConnectionFactory()
            {
                HostName = "192.168.1.3",
                UserName = "root",
                Password = "root",
                Port = 5672,
                AuthMechanisms = new List<IAuthMechanismFactory>() { default },
                TopologyRecoveryExceptionHandler = default,
                Uri = new Uri("amqp://root:root@192.168.1.3:5672"),
            }
        };
        return EventBusFactory.Create(conf, sp);
    });
}