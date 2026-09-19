using MediatR;
using Zentric.Application.Common.Models;
using Zentric.Application.Orders.Ports;
using Zentric.Domain.Orders;

namespace Zentric.Application.Orders.Commands
{
    public record CreateCartCommand(Guid BuyerId) : IRequest<Result<Guid>>;

    public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, Result<Guid>>
    {
        private readonly ICustomerOrderRepository _repository;

        public CreateCartCommandHandler(ICustomerOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var order = new CustomerOrder(request.BuyerId);
            
            await _repository.AddAsync(order, cancellationToken);
            
            return Result<Guid>.Success(order.Id);
        }
    }
}
