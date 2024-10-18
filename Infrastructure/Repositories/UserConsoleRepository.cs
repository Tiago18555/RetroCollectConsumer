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

    public async Task<UserConsole> AddAsync(UserConsole user)
    {
        await _context.UserConsoles.AddAsync(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(x => x.Console).LoadAsync();
        await _context.Entry(user).Reference(x => x.User).LoadAsync();
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public bool Any(Func<UserConsole, bool> predicate)
    {
        return _context
            .UserConsoles
            .AsNoTracking()
            .Any(predicate);
    }

    public async Task<bool> DeleteAsync(UserConsole user)
    {
        _context.UserConsoles.Remove(user);
        await _context.SaveChangesAsync();

        return ! await _context.UserConsoles.AnyAsync(x => x.UserConsoleId == user.UserConsoleId); //NONE MATCH
    }

    public async Task<UserConsole> SingleOrDefaultAsync(Func<UserConsole, bool> predicate)
    {
        return await _context
            .UserConsoles
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<UserConsole> UpdateAsync(UserConsole user)
    {
        _context.UserConsoles.Update(user);
        await _context.Entry(user).Reference(x => x.User).LoadAsync();
        await _context.SaveChangesAsync();

        return user;
    }
}
