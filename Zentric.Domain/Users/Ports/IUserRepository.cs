
using Zentric.Domain.Users.ValueObjects;

namespace Zentric.Domain.Users.Ports
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(Email email);
        Task AddAsync(User user); //! Reglas de unicidad de email 
        Task UpdateAsync(User user); //! Reglas de unicidad de email 
    }
}