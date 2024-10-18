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

    public async Task<UserCollection> AddAsync(UserCollection user)
    {
        await _context.UserCollections.AddAsync(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).Reference(x => x.Game).Load();
        _context.Entry(user).Reference(x => x.User).Load();
        _context.Entry(user).State = EntityState.Detached; 

        return user;
    }

    public bool Any(Func<UserCollection, bool> predicate)
    {
        return _context
            .UserCollections
            .AsNoTracking()
            .Any(predicate);
    }

    public async Task<bool> DeleteAsync(UserCollection user)
    {
        _context.UserCollections.Remove(user);
        await _context.SaveChangesAsync();

        return !_context.UserCollections.Any(x => x.UserCollectionId == user.UserCollectionId); //NONE MATCH
    }

    public async Task<UserCollection> SingleOrDefaultAsync(Func<UserCollection, bool> predicate)
    {
        return await _context
            .UserCollections
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<UserCollection> UpdateAsync(UserCollection user)
    {
        _context.UserCollections.Update(user);
        _context.Entry(user).Reference(x => x.Game).Load();
        _context.Entry(user).Reference(x => x.User).Load();
        await _context.SaveChangesAsync();

        return user;
    }
}
