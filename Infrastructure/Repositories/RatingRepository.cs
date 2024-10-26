using System.Linq.Expressions;
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
    

    public async Task<Rating> AddAsync(Rating rating, CancellationToken cts)
    {
        await _context.Ratings.AddAsync(rating, cts);
        await _context.SaveChangesAsync(cts);
        await _context.Entry(rating).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(rating).Reference(x => x.User).LoadAsync(cts);
        _context.Entry(rating).State = EntityState.Detached;

        return rating;
    }    

    public async Task<bool> AnyAsync(Expression<Func<Rating, bool>> predicate, CancellationToken cts)
    {
        return await _context
            .Ratings
            .AsNoTracking()
            .AnyAsync(predicate, cts);
    }


    public async Task<bool> DeleteAsync(Rating rating, CancellationToken cts)
    {
        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync(cts);

        return !await _context.Ratings.AnyAsync(x => x.RatingId == rating.RatingId, cts);
    }

    public async Task<Rating> SingleOrDefaultAsync(Func<Rating, bool> predicate, CancellationToken cts)
    {
        return await _context
            .Ratings
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefaultAsync(cts);
    }

    public async Task<Rating> UpdateAsync(Rating rating, CancellationToken cts)
    {
        _context.Ratings.Update(rating);
        _context.Entry(rating).State = EntityState.Modified;
        await _context.Entry(rating).Reference(x => x.Game).LoadAsync(cts);
        await _context.Entry(rating).Reference(x => x.User).LoadAsync(cts);
        await _context.SaveChangesAsync(cts);
        _context.Entry(rating).State = EntityState.Detached;

        return rating;
    }
}
