using System.Linq.Expressions;
using Console = Domain.Entities.Console;

namespace Domain.Repositories;

public interface IConsoleRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Console> AddAsync(Console game, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<Console, bool>> predicate, CancellationToken cts);
}
