using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly DataContext _context;

    public GameRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Game> AddAsync(Game game, CancellationToken cts)
    {
        await _context.Games.AddAsync(game, cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(game).State = EntityState.Detached;

        return game;
    }

    public async Task<bool> AnyAsync(Expression<Func<Game, bool>> predicate, CancellationToken cts)
    {
        return await _context.Games.AsNoTracking().AnyAsync(predicate, cts);
    }
}
