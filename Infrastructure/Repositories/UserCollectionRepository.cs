using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserCollectionRepository : IUserCollectionRepository
{
    private readonly DataContext _context;

    public UserCollectionRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserCollection> AddAsync(UserCollection user, CancellationToken cts)
    {
        await _context.UserCollections.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached; 

        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<UserCollection, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .UserCollections
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<bool> DeleteAsync(UserCollection user, CancellationToken cts)
    {
        _context.UserCollections.Remove(user);
        await _context.SaveChangesAsync(cts);

        return !await _context.UserCollections.AnyAsync(x => x.UserCollectionId == user.UserCollectionId, cts); //NONE MATCH
    }

    public async Task<UserCollection> SingleOrDefaultAsync(Expression<Func<UserCollection, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .UserCollections
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<UserCollection> UpdateAsync(UserCollection user, CancellationToken cts)
    {
        _context.UserCollections.Update(user);
        await _context.Entry(user).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
}
