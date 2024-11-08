using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IConsoleCollectionItemRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<ConsoleCollectionItem> AddAsync(ConsoleCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(ConsoleCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<ConsoleCollectionItem, bool>> predicate, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<ConsoleCollectionItem> SingleOrDefaultAsync(Expression<Func<ConsoleCollectionItem, bool>> predicate, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<ConsoleCollectionItem> UpdateAsync(ConsoleCollectionItem user, CancellationToken cts);
}
