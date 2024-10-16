﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Linq.Expressions;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        _context.Entry(user).State = EntityState.Detached;

        return user;
    }

    public async Task<List<T>> GetAll<T>(Expression<Func<User, T>> predicate)
    {
        return await _context.Users
            .AsNoTracking()
            .Select(predicate)
            .ToListAsync();
    }

    public bool Any(Func<User, bool> predicate)
    {
        return _context.Users.AsNoTracking().Any(predicate);
    }

    public User SingleOrDefault(Func<User, bool> predicate)
    {
        return _context
            .Users
            .Where(predicate)
            .AsQueryable()
            .AsNoTracking()
            .SingleOrDefault();
    }

    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        _context.SaveChanges();

        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<T>> GetAllComputersByUser<T>(Guid userId, Expression<Func<UserComputer, T>> predicate)
    {
        return await _context.UserComputers
            .Include(x => x.Computer)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(predicate)
            .ToListAsync();
    }

    public async Task<List<T>> GetAllComputersByUser<T>(Guid userId, Expression<Func<UserComputer, T>> predicate, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        return await _context.UserComputers
                    .Include(x => x.Computer)
                    .AsNoTracking()
                    .Where(x => x.UserId == userId)
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(predicate)
                    .ToListAsync();
    }

    public async Task<List<T>> GetAllConsolesByUser<T>(Guid userId, Expression<Func<UserConsole, T>> predicate)
    {
        return await Task.FromResult(_context.UserConsoles
            .Include(x => x.Console)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(predicate)
            .ToList());
    }

    public async Task<List<T>> GetAllConsolesByUser<T>(Guid userId, Expression<Func<UserConsole, T>> predicate, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        return await Task.FromResult(_context.UserConsoles
            .Include(x => x.Console)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Skip(offset)
            .Take(pageSize)
            .Select(predicate)
            .ToList());
    }

    public async Task<List<T>> GetAllCollectionsByUser<T>(Guid userId, Expression<Func<UserCollection, T>> predicate)
    {
        return await Task.FromResult(_context.UserCollections
             .Include(x => x.Game)
             .AsNoTracking()
             .Where(x => x.UserId == userId)
             .Select(predicate)
             .ToList());
    }

    public async Task<List<T>> GetAllCollectionsByUser<T>(Guid userId, Expression<Func<UserCollection, T>> predicate, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        return await Task.FromResult(_context.UserCollections
             .Include(x => x.Game)
             .AsNoTracking()
             .Where(x => x.UserId == userId)
             .Skip(offset)
             .Take(pageSize)
             .Select(predicate)
             .ToList());
    }
}
