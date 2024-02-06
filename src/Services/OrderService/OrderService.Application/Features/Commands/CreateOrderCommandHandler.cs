﻿using EventBus.Base.Abstract;
using MediatR;
using OrderService.Application.IntegrationEvents;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var addr = new Address(request.City,request.Street,request.State,request.Country,request.ZipCode);

            Order dbOrder = new(request.UserName, addr, request.CardTypeId, request.CardNumber, request.CardSecurityNumber,
                                request.CardHolderName, request.CardExpiration,null);

            request.OrderItems.ToList().ForEach(i => dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Units));
            
            await _orderRepository.AddAsync(dbOrder);
            await _orderRepository.unitOfWork.SaveChangesAsync(cancellationToken);
            
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName);

            _eventBus.Publish(orderStartedIntegrationEvent);

            return true;
        }
    }
}
