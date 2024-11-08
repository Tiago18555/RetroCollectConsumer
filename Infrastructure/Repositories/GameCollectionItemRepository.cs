using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GameCollectionItemRepository : IGameCollectionItemRepository
{
    private readonly DataContext _context;

    public GameCollectionItemRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<GameCollectionItem> AddAsync(GameCollectionItem user, CancellationToken cts)
    {
        await _context.GameCollectionItems.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached; 

        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<GameCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .GameCollectionItems
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<bool> DeleteAsync(GameCollectionItem user, CancellationToken cts)
    {
        _context.GameCollectionItems.Remove(user);
        await _context.SaveChangesAsync(cts);

        return !await _context.GameCollectionItems.AnyAsync(x => x.Id == user.Id, cts); //NONE MATCH
    }

    public async Task<GameCollectionItem> SingleOrDefaultAsync(Expression<Func<GameCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .GameCollectionItems
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<GameCollectionItem> UpdateAsync(GameCollectionItem user, CancellationToken cts)
    {
        _context.GameCollectionItems.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.Entry(user).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
}
