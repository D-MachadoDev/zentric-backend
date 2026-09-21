
using Zentric.Domain.Users.ValueObjects;

namespace Zentric.Domain.Users.Ports
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(Email email);
        Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> IsIdentityDocumentUniqueAsync(string identityDocument, CancellationToken cancellationToken = default);
        Task AddAsync(User user); //! Reglas de unicidad de email 
        Task UpdateAsync(User user); //! Reglas de unicidad de email 
    }
}