using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zentric.Domain.Warehouses.Ports
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
        Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    }
}
