using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using RabbitMQ.Client;

namespace PaymentService.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection serviceRegistration(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddLogging(conf =>
            {
                conf.AddConsole();
            });
            services.AddTransient<OrderStartedIntegrationEventHandler>();

            services.AddSingleton<IEventBus>(sp =>
            {
                var conf = new EventBusConfig()
                {
                    ConnectionRetryCount = int.Parse(configuration["EventBusConfiguration:RetryCount"]),
                    EventNameSuffix = configuration["EventBusConfiguration:EventNameSuffix"],
                    SubscriberClientAppName = configuration["EventBusConfiguration:ServiceName"],
                    EventBusType = EventBusType.RabbitMQ,
                    Connection = new ConnectionFactory()
                    {
                        AuthMechanisms = new List<IAuthMechanismFactory>() { default },
                        TopologyRecoveryExceptionHandler = default,
                        Uri = new Uri(configuration["EventBusConfiguration:Host"]),
                    }
                };
                return EventBusFactory.Create(conf, sp);
            });

            return services;
        }
    }
}
