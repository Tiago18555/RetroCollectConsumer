using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IUserConsoleRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserConsole> AddAsync(UserConsole user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(UserConsole user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<UserConsole, bool>> predicate, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<UserConsole> SingleOrDefaultAsync(Func<UserConsole, bool> predicate, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<UserConsole> UpdateAsync(UserConsole user, CancellationToken cts);
}
