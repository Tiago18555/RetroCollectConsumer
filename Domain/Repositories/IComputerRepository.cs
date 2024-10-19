using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IComputerRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Computer> AddAsync(Computer game, CancellationToken cts);

    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> AnyAsync(Expression<Func<Computer, bool>> predicate, CancellationToken cts);
}
