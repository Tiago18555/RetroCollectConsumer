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

    public async Task<Console> AddAsync(Console console)
    {
        await _context.Consoles.AddAsync(console);
        await _context.SaveChangesAsync();
        _context.Entry(console).State = EntityState.Detached;

        return console;
    }

    public bool Any(Func<Console, bool> predicate)
    {
        return _context.Consoles.AsNoTracking().Any(predicate);
    }
}
