using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories;

public interface IWishlistRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Wishlist> AddAsync(Wishlist wishlist);

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><see langword="true" /> if the entity has deleted successfully</returns>
    Task<bool> DeleteAsync(Wishlist wishlist);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<Wishlist, bool> predicate);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<Wishlist> SingleOrDefaultAsync(Func<Wishlist, bool> predicate);
}
