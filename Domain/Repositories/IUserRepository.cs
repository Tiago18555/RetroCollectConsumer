using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<User> AddAsync(User user);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<User, bool> predicate);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<User> SingleOrDefaultAsync(Func<User, bool> predicate);

    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<User> UpdateAsync(User user);
}
