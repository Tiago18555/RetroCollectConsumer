using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserConsoleRepository : IUserConsoleRepository
{
    private readonly DataContext _context;

    public UserConsoleRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserConsole> AddAsync(UserConsole user, CancellationToken cts)
    {
        await _context.UserConsoles.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Console).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
    public async Task<bool> AnyAsync(Expression<Func<UserConsole, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .UserConsoles
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }


    public async Task<bool> DeleteAsync(UserConsole user, CancellationToken cts)
    {
        _context.UserConsoles.Remove(user);
        await _context.SaveChangesAsync(cts);

        return ! await _context.UserConsoles.AnyAsync(x => x.UserConsoleId == user.UserConsoleId, cts); //NONE MATCH
    }

    public async Task<UserConsole> SingleOrDefaultAsync(Func<UserConsole, bool> predicate, CancellationToken cts)
    {
        return await _context
            .UserConsoles
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<UserConsole> UpdateAsync(UserConsole user, CancellationToken cts)
    {
        _context.UserConsoles.Update(user);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);

        return user;
    }
}
