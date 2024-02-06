using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;

namespace OrderService.Api.Extensions.Registration.EventHandlerRegistration
{
    public static class EventHandlerRegistration
    {
        public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<OrderCreatedIntegrationEventHandler>();
            
            return services;
        }
    }
}
