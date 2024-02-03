using BasketService.Api.Core.Applicaton.Repository;
using BasketService.Api.Core.Applicaton.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Event;
using EventBus.Base.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository repository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            _repository = repository;
            _identityService = identityService;
            _eventBus = eventBus;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket Service is Up and Running");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _repository.GetBasketAsync(id);
            if(basket == null)
            {
                
                return Ok(new CustomerBasket());
            }
            return Ok(basket);
            
        }

        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public  async Task<ActionResult<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket)
        {
            return Ok(await _repository.UpdateBasketAsync(basket));
        }

        [Route("additem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<ActionResult> AddToItemBasket(BasketItem item)
        {
            var userId = _identityService.GetUserName().ToString();

            var basket = await _repository.GetBasketAsync(userId);

            if(basket == null) basket = new CustomerBasket(userId);
            
            basket.Items.Add(item);

            await _repository.UpdateBasketAsync(basket);

            return Ok();
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync(BasketCheckout basketCheckout )
        {
            var userId = basketCheckout.Buyer;

            var basket = await _repository.GetBasketAsync(userId);

            if(basket == null)
            {
                return BadRequest();
            }

            var userName = _identityService.GetUserName();
            var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street, basketCheckout.State,
                basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName, basketCheckout.CardExpiration,
                basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basket);

            try
            {
                // start OrderApi to OrderStarted event process
                _eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex ,"ERROR Publishing integration Event : {event} from {basketService.App}", eventMessage.Id);
                
                throw;
            }

            return Accepted();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await _repository.DeleteBasketAsync(id);
        }


    }
}
