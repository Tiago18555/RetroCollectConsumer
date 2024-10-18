using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public bool Any(Func<User, bool> predicate)
    {
        return _context.Users.AsNoTracking().Any(predicate);
    }

    public async Task<User> SingleOrDefaultAsync(Func<User, bool> predicate)
    {
        return await _context
            .Users
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return user;
    }
}
