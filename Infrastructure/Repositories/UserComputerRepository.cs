using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserComputerRepository : IUserComputerRepository
{
    private readonly DataContext _context;

    public UserComputerRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserComputer> AddAsync(UserComputer user)
    {
        _context.UserComputers.Add(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(x => x.Computer).LoadAsync();
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public bool Any(Func<UserComputer, bool> predicate)
    {
        return _context
            .UserComputers
            .AsNoTracking()
            .Any(predicate);
    }

    public async Task<bool> DeleteAsync(UserComputer user)
    {
        _context.UserComputers.Remove(user);
        await _context.SaveChangesAsync();

        return !await _context.UserComputers.AnyAsync(x => x.UserComputerId == user.UserComputerId); //NONE MATCH
    }

    public async Task<UserComputer> SingleOrDefaultAsync(Func<UserComputer, bool> predicate)
    {
        return await _context
            .UserComputers
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<UserComputer> UpdateAsync(UserComputer user)
    {
        _context.UserComputers.Update(user);;
        _context.Entry(user).Reference(x => x.User).Load();
        await _context.SaveChangesAsync();

        return user;
    }
}
