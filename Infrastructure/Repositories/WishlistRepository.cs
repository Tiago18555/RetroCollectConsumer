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

    public async Task<Wishlist> AddAsync(Wishlist wishlist)
    {
        await _context.Wishlists.AddAsync(wishlist);
        await _context.SaveChangesAsync();
        _context.Entry(wishlist).Reference(x => x.Game).Load();
        _context.Entry(wishlist).Reference(x => x.User).Load();
        _context.Entry(wishlist).State = EntityState.Detached;

        return wishlist;
    }

    public bool Any(Func<Wishlist, bool> predicate)
    {
        return _context
            .Wishlists
            .AsNoTracking()
            .Any(predicate);
    }

    public async Task<bool> DeleteAsync(Wishlist rating)
    {
        _context.Wishlists.Remove(rating);
        await _context.SaveChangesAsync();

        var res = !await _context.Wishlists.AnyAsync(x => x.WishlistId == rating.WishlistId);

        return res;
    }
    public async Task<Wishlist> SingleOrDefaultAsync(Func<Wishlist, bool> predicate)
    {
        return await _context
            .Wishlists
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }
}
