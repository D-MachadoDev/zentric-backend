using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zentric.Domain.Orders.Ports;
using Zentric.Domain.Orders;
using Zentric.Infrastructure.Persistence.Mappers;

namespace Zentric.Infrastructure.Persistence.Repositories
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly ZentricDbContext _dbContext;

        public CustomerOrderRepository(ZentricDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbModel = await _dbContext.CustomerOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            return dbModel == null ? null : CustomerOrderMapper.ToDomain(dbModel);
        }

        public Task AddAsync(CustomerOrder order, CancellationToken cancellationToken = default)
        {
            var dbModel = CustomerOrderMapper.ToDbModel(order);
            _dbContext.CustomerOrders.Add(dbModel);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(CustomerOrder order, CancellationToken cancellationToken = default)
        {
            var dbModel = CustomerOrderMapper.ToDbModel(order);
            _dbContext.CustomerOrders.Update(dbModel);
            return Task.CompletedTask;
        }
    }
}
