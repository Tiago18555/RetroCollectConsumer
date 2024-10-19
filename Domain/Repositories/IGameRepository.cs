using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IGameRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Game> AddAsync(Game game, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<Game, bool>> predicate, CancellationToken cts);
}
