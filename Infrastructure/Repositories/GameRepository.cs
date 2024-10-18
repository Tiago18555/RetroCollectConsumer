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

    public async Task<Game> AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
        _context.Entry(game).State = EntityState.Detached;

        return game;
    }

    public bool Any(Func<Game, bool> predicate)
    {
        return _context
            .Games
            .AsNoTracking()
            .Any(predicate);
    }
}
