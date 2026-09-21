using MediatR;
using Zentric.Application.Common.Models;
using Zentric.Domain.Logistics.Ports;
using Zentric.Domain.Logistics;

namespace Zentric.Application.Logistics.Commands
{
    public record CreateFulfillmentOrderCommand(Guid CustomerOrderId, Guid VendorId) : IRequest<Result<Guid>>;

    public class CreateFulfillmentOrderCommandHandler : IRequestHandler<CreateFulfillmentOrderCommand, Result<Guid>>
    {
        private readonly IFulfillmentOrderRepository _repository;

        public CreateFulfillmentOrderCommandHandler(IFulfillmentOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateFulfillmentOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new FulfillmentOrder(request.CustomerOrderId, request.VendorId);
            await _repository.AddAsync(order, cancellationToken);
            return Result<Guid>.Success(order.Id);
        }
    }
}
