using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IComputerCollectionItemRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<ComputerCollectionItem> AddAsync(ComputerCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(ComputerCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<ComputerCollectionItem, bool>> predicate, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<ComputerCollectionItem> SingleOrDefaultAsync(Expression<Func<ComputerCollectionItem, bool>> predicate, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<ComputerCollectionItem> UpdateAsync(ComputerCollectionItem user, CancellationToken cts);
}
