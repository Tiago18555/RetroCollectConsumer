using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Game = Domain.Entities.Game;
using Domain.Repositories;
using CrossCutting.Providers;
using CrossCutting;
using Application.IgdbIntegrationOperations.SearchGame;
using Domain.Broker;
using System.Text.Json;

namespace Application.Processors.RatingOperations.AddRating;

public class AddRatingProcessor : IRequestProcessor
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISearchGame _searchGameService;

    public AddRatingProcessor (
        IRatingRepository repository, 
        IGameRepository gameRepository, 
        IDateTimeProvider dateTimeProvider, 
        ISearchGame searchGameService
    )
    {
        _ratingRepository = repository;
        _gameRepository = gameRepository;
        _dateTimeProvider = dateTimeProvider;
        _searchGameService = searchGameService;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddRatingRequest>(field);
        var res = await AddRatingAsync(request, cts);
    }

    private async Task<bool> AddRatingAsync(AddRatingRequest requestBody, CancellationToken cts)
    {
        try
        {
            if (!await _gameRepository.AnyAsync(g => g.GameId == requestBody.GameId, cts))
            {                    
                var result = await _searchGameService.RetrieveGameInfoAsync(requestBody.GameId);

                var gameInfo = result.Single();

                Game game = new()
                {
                    GameId = gameInfo.GameId,
                    Genres = gameInfo.Genres,
                    Description = gameInfo.Description ?? "",
                    Summary = gameInfo.Summary ?? "",
                    ImageUrl = gameInfo.Cover ?? "",
                    Title = gameInfo.Title ?? "",
                    ReleaseYear = gameInfo.FirstReleaseDate
                };


                await _gameRepository.AddAsync(game, cts);                    
            }

            var newRating = requestBody.MapObjectTo(new Rating());

            newRating.CreatedAt = _dateTimeProvider.UtcNow;
            newRating.UserId = requestBody.UserId;

            var res = await _ratingRepository.AddAsync(newRating, cts);

            StdOut.Info("rating added");
            return true;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidOperationException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (Exception e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
    }
}
