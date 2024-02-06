using BasketService.Api.Core.Applicaton.Repository;
using BasketService.Api.Core.Applicaton.Services;
using BasketService.Api.Infrastructure.Repository;
using EventBus.Base.Abstract;
using EventBus.Base;
using EventBus.Factory;
using RabbitMQ.Client;
using BasketService.Api.IntegrationEvents.EventHandler;

namespace BasketService.Api.Extensions
{
    public static class EventBusRegistration
    {
        public static IServiceCollection EventBusServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBasketRepository, RedisBasketRepository>();

            services.AddTransient<OrderCreatedIntegrationEventHandler>();


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
