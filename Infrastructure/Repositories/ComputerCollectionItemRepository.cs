using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ComputerCollectionItemRepository : IComputerCollectionItemRepository
{
    private readonly DataContext _context;

    public ComputerCollectionItemRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ComputerCollectionItem> AddAsync(ComputerCollectionItem user, CancellationToken cts)
    {
        await _context.ComputerCollectionItems.AddAsync(user, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(user).Reference(x => x.Computer).LoadAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public async Task<bool> AnyAsync(Expression<Func<ComputerCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .ComputerCollectionItems
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }

    public async Task<bool> DeleteAsync(ComputerCollectionItem user, CancellationToken cts)
    {
        _context.ComputerCollectionItems.Remove(user);
        await _context.SaveChangesAsync(cts);

        return !await _context.ComputerCollectionItems.AnyAsync(x => x.Id == user.Id, cts); //NONE MATCH
    }

    public async Task<ComputerCollectionItem> SingleOrDefaultAsync(Expression<Func<ComputerCollectionItem, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .ComputerCollectionItems
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<ComputerCollectionItem> UpdateAsync(ComputerCollectionItem user, CancellationToken cts)
    {
        _context.ComputerCollectionItems.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.Entry(user).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }
}
