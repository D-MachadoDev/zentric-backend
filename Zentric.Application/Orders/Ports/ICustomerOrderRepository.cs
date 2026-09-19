using Zentric.Domain.Orders;

namespace Zentric.Application.Orders.Ports
{
    public interface ICustomerOrderRepository
    {
        Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(CustomerOrder order, CancellationToken cancellationToken = default);
        Task UpdateAsync(CustomerOrder order, CancellationToken cancellationToken = default);
    }
}
