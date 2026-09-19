using Microsoft.EntityFrameworkCore;
using Zentric.Application.Logistics.Ports;
using Zentric.Domain.Logistics;
using Zentric.Infrastructure.Persistence;

namespace Zentric.Infrastructure.Repositories
{
    public class FulfillmentOrderRepository : IFulfillmentOrderRepository
    {
        private readonly ZentricDbContext _dbContext;

        public FulfillmentOrderRepository(ZentricDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(FulfillmentOrder order, CancellationToken cancellationToken = default)
        {
            await _dbContext.FulfillmentOrders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<FulfillmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.FulfillmentOrders
                .Include(f => f.Shipments)
                .SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(FulfillmentOrder order, CancellationToken cancellationToken = default)
        {
            _dbContext.FulfillmentOrders.Update(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
