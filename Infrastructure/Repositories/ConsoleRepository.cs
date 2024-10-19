using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Console = Domain.Entities.Console;

namespace Infrastructure.Repositories;

public class ConsoleRepository : IConsoleRepository
{
    private readonly DataContext _context;

    public ConsoleRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Console> AddAsync(Console console, CancellationToken cts)
    {
        await _context.Consoles.AddAsync(console, cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(console).State = EntityState.Detached;

        return console;
    }

    public async Task<bool> AnyAsync(Expression<Func<Console, bool>> predicate, CancellationToken cts)
    {
        return await _context.Consoles.AsNoTracking().AnyAsync(predicate, cts);
    }
}
