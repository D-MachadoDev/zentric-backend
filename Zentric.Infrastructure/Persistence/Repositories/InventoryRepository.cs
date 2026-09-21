using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zentric.Domain.Inventories.Ports;
using Zentric.Domain.Inventories;
using Zentric.Infrastructure.Persistence.Mappers;

namespace Zentric.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ZentricDbContext _dbContext;

        public InventoryRepository(ZentricDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbModel = await _dbContext.Inventories
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            return dbModel == null ? null : InventoryMapper.ToDomain(dbModel);
        }

        public Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            var dbModel = InventoryMapper.ToDbModel(inventory);
            _dbContext.Inventories.Add(dbModel);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            var dbModel = InventoryMapper.ToDbModel(inventory);
            _dbContext.Inventories.Update(dbModel);
            return Task.CompletedTask;
        }

        public async Task<Inventory?> GetByVariantAndWarehouseAsync(Guid variantId, Guid warehouseId, CancellationToken cancellationToken = default)
        {
            var dbModel = await _dbContext.Inventories
                .FirstOrDefaultAsync(i => i.VariantId == variantId && i.WarehouseId == warehouseId, cancellationToken);
            return dbModel == null ? null : InventoryMapper.ToDomain(dbModel);
        }

        public async Task<int> GetTotalAvailableStockAsync(Guid variantId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Inventories
                .Where(i => i.VariantId == variantId)
                .SumAsync(i => i.AvailableQuantity, cancellationToken);
        }

        public async Task<IEnumerable<Inventory>> GetByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default)
        {
            var dbModels = await _dbContext.Inventories
                .Where(i => i.VariantId == variantId)
                .ToListAsync(cancellationToken);
            return dbModels.Select(InventoryMapper.ToDomain);
        }
    }
}
