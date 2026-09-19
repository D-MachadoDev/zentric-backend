using Microsoft.EntityFrameworkCore;
using Zentric.Application.Orders.Ports;
using Zentric.Domain.Orders;
using Zentric.Infrastructure.Persistence;

namespace Zentric.Infrastructure.Repositories
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly ZentricDbContext _dbContext;

        public CustomerOrderRepository(ZentricDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(CustomerOrder order, CancellationToken cancellationToken = default)
        {
            await _dbContext.CustomerOrders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.CustomerOrders
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(CustomerOrder order, CancellationToken cancellationToken = default)
        {
            _dbContext.CustomerOrders.Update(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
