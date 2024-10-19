using System.Linq.Expressions;
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

    public async Task<Computer> AddAsync(Computer computer, CancellationToken cts)
    {
        await _context.Computers.AddAsync(computer, cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(computer).State = EntityState.Detached;

        return computer;
    }

    public async Task<bool> AnyAsync(Expression<Func<Computer, bool>> predicate, CancellationToken cts)
    {
        return await _context.Computers.AsNoTracking().AnyAsync(predicate, cts);
    }
}
