using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConsoleCollectionItemRepository : IConsoleCollectionItemRepository
{
    private readonly DataContext _context;

    public ConsoleCollectionItemRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ConsoleCollectionItem> AddAsync(ConsoleCollectionItem user, CancellationToken cts)
    {
        await _context.ConsoleCollectionItems.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Console).LoadAsync(cts);
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
    public async Task<bool> AnyAsync(Expression<Func<ConsoleCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .ConsoleCollectionItems
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }


    public async Task<bool> DeleteAsync(ConsoleCollectionItem user, CancellationToken cts)
    {
        _context.ConsoleCollectionItems.Remove(user);
        await _context.SaveChangesAsync(cts);

        return ! await _context.ConsoleCollectionItems.AnyAsync(x => x.Id == user.Id, cts); //NONE MATCH
    }

    public async Task<ConsoleCollectionItem> SingleOrDefaultAsync(Expression<Func<ConsoleCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .ConsoleCollectionItems
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<ConsoleCollectionItem> UpdateAsync(ConsoleCollectionItem user, CancellationToken cts)
    {
        _context.ConsoleCollectionItems.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
}
