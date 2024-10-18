using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ComputerRepository : IComputerRepository
{
    private readonly DataContext _context;

    public ComputerRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Computer> AddAsync(Computer computer)
    {
        await _context.Computers.AddAsync(computer);
        await _context.SaveChangesAsync();
        _context.Entry(computer).State = EntityState.Detached;

        return computer;
    }

    public bool Any(Func<Computer, bool> predicate)
    {
        return _context.Computers.AsNoTracking().Any(predicate);
    }
}
