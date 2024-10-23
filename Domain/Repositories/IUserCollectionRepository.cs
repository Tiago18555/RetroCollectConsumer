using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IUserCollectionRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserCollection> AddAsync(UserCollection user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(UserCollection user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<UserCollection, bool>> predicate, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserCollection> UpdateAsync(UserCollection user, CancellationToken cts);
    
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<UserCollection> SingleOrDefaultAsync(Expression<Func<UserCollection, bool>> predicate, CancellationToken cts);
}
