using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace Infrastructure.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly DataContext _context;

    public WishlistRepository(DataContext context) =>
        _context = context;

    public async Task<Wishlist> AddAsync(Wishlist wishlist, CancellationToken cts)
    {
        await _context.Wishlists.AddAsync(wishlist, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(wishlist).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(wishlist).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(wishlist).State = EntityState.Detached;

        return wishlist;
    }

    public async Task<bool> AnyAsync(Expression<Func<Wishlist, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .Wishlists
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<bool> DeleteAsync(Wishlist rating, CancellationToken cts)
    {
        _context.Wishlists.Remove(rating);
        await _context.SaveChangesAsync(cts);

        var res = !await _context.Wishlists.AnyAsync(x => x.WishlistId == rating.WishlistId, cts);

        return res;
    }
    public async Task<Wishlist> SingleOrDefaultAsync(Func<Wishlist, bool> predicate, CancellationToken cts)
    {
        return await _context
            .Wishlists
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }
}
