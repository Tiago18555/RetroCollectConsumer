using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace Infrastructure.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly DataContext _context;

    public RatingRepository(DataContext context) =>        
        _context = context;
    

    public async Task<Rating> AddAsync(Rating rating)
    {
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
        await _context.Entry(rating).Reference(x => x.Game).LoadAsync();
        await _context.Entry(rating).Reference(x => x.User).LoadAsync();
        _context.Entry(rating).State = EntityState.Detached;

        return rating;
    }    

    public bool Any(Func<Rating, bool> predicate)
    {
        return _context
            .Ratings
            .AsNoTracking()
            .Any(predicate);
    }


    public async Task<bool> DeleteAsync(Rating rating)
    {
        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();

        return !_context.Ratings.Any(x => x.RatingId == rating.RatingId);
    }

    public async Task<Rating> SingleOrDefaultAsync(Func<Rating, bool> predicate)
    {
        return await _context
            .Ratings
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<Rating> UpdateAsync(Rating rating)
    {
        _context.Ratings.Update(rating);
        _context.Entry(rating).Reference(x => x.Game).Load();
        _context.Entry(rating).Reference(x => x.User).Load();
        await _context.SaveChangesAsync();

        return rating;
    }
}
