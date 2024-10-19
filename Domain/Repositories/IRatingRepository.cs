using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories;

public interface IRatingRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Rating> AddAsync(Rating rating, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(Rating rating, CancellationToken cts);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Rating> UpdateAsync(Rating rating, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<Rating, bool>> predicate, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<Rating> SingleOrDefaultAsync(Func<Rating, bool> predicate, CancellationToken cts);
}
