using Domain.Entities;

namespace Domain.Repositories;

public interface IUserCollectionRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserCollection> AddAsync(UserCollection user);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(UserCollection user);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<UserCollection, bool> predicate);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserCollection> UpdateAsync(UserCollection user);
}
