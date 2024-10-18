using Domain.Entities;

namespace Domain.Repositories;

public interface IUserConsoleRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserConsole> AddAsync(UserConsole user);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(UserConsole user);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<UserConsole, bool> predicate);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<UserConsole> SingleOrDefaultAsync(Func<UserConsole, bool> predicate);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserConsole> UpdateAsync(UserConsole user);
}
