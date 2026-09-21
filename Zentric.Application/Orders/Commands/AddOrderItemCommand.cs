using MediatR;
using Zentric.Application.Common.Models;
using Zentric.Domain.Orders.Ports;
using Zentric.Domain.Orders.Enums;
using Zentric.Domain.Products.ValueObjects;

using Zentric.Domain.Inventories.Ports;

namespace Zentric.Application.Orders.Commands
{
    public record AddOrderItemCommand(Guid OrderId, Guid VariantId, int Quantity, decimal UnitPrice, string Currency) : IRequest<Result>;

    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result>
    {
        private readonly ICustomerOrderRepository _orderRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public AddOrderItemCommandHandler(ICustomerOrderRepository orderRepository, IInventoryRepository inventoryRepository)
        {
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<Result> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return Result.Failure("Order not found.");
            }

            if (order.Status != OrderStatus.Cart)
            {
                return Result.Failure("Items can only be added while the order is in the Cart status.");
            }

            var totalAvailable = await _inventoryRepository.GetTotalAvailableStockAsync(request.VariantId, cancellationToken);
            if (totalAvailable < request.Quantity)
            {
                return Result.Failure("Not enough available stock.");
            }

            var money = new Money(request.UnitPrice, request.Currency);

            order.AddItem(request.VariantId, request.Quantity, money);
            await _orderRepository.UpdateAsync(order, cancellationToken);
            return Result.Success();
        }
    }
}
