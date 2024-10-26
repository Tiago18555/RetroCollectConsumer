using System.Linq.Expressions;
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

    public async Task<UserComputer> AddAsync(UserComputer user, CancellationToken cts)
    {
        await _context.UserComputers.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Computer).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<UserComputer, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .UserComputers
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<bool> DeleteAsync(UserComputer user, CancellationToken cts)
    {
        _context.UserComputers.Remove(user);
        await _context.SaveChangesAsync(cts);

        return !await _context.UserComputers.AnyAsync(x => x.UserComputerId == user.UserComputerId, cts); //NONE MATCH
    }

    public async Task<UserComputer> SingleOrDefaultAsync(Expression<Func<UserComputer, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .UserComputers
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<UserComputer> UpdateAsync(UserComputer user, CancellationToken cts)
    {
        _context.UserComputers.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
}
