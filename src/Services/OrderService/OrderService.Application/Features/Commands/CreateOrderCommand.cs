using MediatR;
using OrderService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Commands
{
    public class CreateOrderCommand : IRequest<bool>
    {
        private readonly List<OrderItemDto> _orderItems;
        public string UserName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }

        public IEnumerable<OrderItemDto> OrderItems => _orderItems;

       

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDto>();
        }

        public CreateOrderCommand(List<BasketItem> basketItems,string userId,string userName,string city, string street,string state,
            string country,string zipcode, string cardNumber,string cardHolderName,DateTime cardExpiration,string cardSecurityNumber,int cardTypeId):this()
        {
            var dtoList = basketItems.Select(item => new OrderItemDto()
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Units = item.Quantity
            });

            _orderItems = dtoList.ToList();

            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            CardExpiration = cardExpiration;
        }


    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
    }
}
