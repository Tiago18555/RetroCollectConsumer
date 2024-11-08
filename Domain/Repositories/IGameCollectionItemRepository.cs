using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IGameCollectionItemRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<GameCollectionItem> AddAsync(GameCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(GameCollectionItem user, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<GameCollectionItem, bool>> predicate, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<GameCollectionItem> UpdateAsync(GameCollectionItem user, CancellationToken cts);
    
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<GameCollectionItem> SingleOrDefaultAsync(Expression<Func<GameCollectionItem, bool>> predicate, CancellationToken cts);
}
