using Domain.Entities;

namespace Domain.Repositories;

public interface IUserComputerRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserComputer> AddAsync(UserComputer user);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(UserComputer user);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<UserComputer, bool> predicate);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<UserComputer> SingleOrDefaultAsync(Func<UserComputer, bool> predicate);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserComputer> UpdateAsync(UserComputer user);
}
