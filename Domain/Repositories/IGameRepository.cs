﻿using Domain.Entities;

namespace Domain.Repositories;

public interface IGameRepository
{
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    /// <returns>The entity found, or <see langword="null" />.</returns>
    Task<Game> AddAsync(Game game);

    /// <exception cref="ArgumentNullException"></exception>
    bool Any(Func<Game, bool> predicate);
}
