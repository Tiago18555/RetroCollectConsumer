using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Domain.Repositories;
using Infrastructure.Data;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user, CancellationToken cts)
    {
        await _context.Users.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .Users
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<User> SingleOrDefaultAsync(Func<User, bool> predicate, CancellationToken cts)
    {
        return await _context
            .Users
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cts)
    {
        _context.Users.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync(cts);

        return user;
    }
}
