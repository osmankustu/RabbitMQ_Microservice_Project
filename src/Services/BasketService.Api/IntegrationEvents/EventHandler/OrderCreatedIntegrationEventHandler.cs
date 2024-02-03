using BasketService.Api.Core.Applicaton.Repository;
using BasketService.Api.IntegrationEvents.Event;
using EventBus.Base.Abstract;

namespace BasketService.Api.IntegrationEvents.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationGenericEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly  IBasketRepository _basketRepository;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(IBasketRepository basketRepository,ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }
        
        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("---- Handling integration event {Id} at BasketService.Api - ({@IntegrationEvent})", @event.Id, @event.UserName);

             await _basketRepository.DeleteBasketAsync(@event.UserId.ToString());
        }
    }
}
