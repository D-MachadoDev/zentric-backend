using MediatR;
using Zentric.Application.Common.Models;
using Zentric.Application.Orders.Ports;
using Zentric.Domain.Orders.Enums;
using Zentric.Domain.Products.ValueObjects;

namespace Zentric.Application.Orders.Commands
{
    public record AddOrderItemCommand(Guid OrderId, Guid VariantId, int Quantity, decimal UnitPrice, string Currency) : IRequest<Result>;

    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result>
    {
        private readonly ICustomerOrderRepository _repository;

        public AddOrderItemCommandHandler(ICustomerOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return Result.Failure("Order not found.");
            }

            // Precondición de negocio explícita: se informa el fallo como Result en lugar de
            // capturar la excepción que lanza el agregado. AGENTS.md §3.2 prohíbe el try-catch
            // genérico en la capa de Aplicación y exige representar los fallos previsibles
            // con Result; las excepciones catastróficas suben al middleware global (§3.4).
            if (order.Status != OrderStatus.Cart)
            {
                return Result.Failure("Items can only be added while the order is in the Cart status.");
            }

            var money = new Money(request.UnitPrice, request.Currency);

            order.AddItem(request.VariantId, request.Quantity, money);
            await _repository.UpdateAsync(order, cancellationToken);
            return Result.Success();
        }
    }
}
