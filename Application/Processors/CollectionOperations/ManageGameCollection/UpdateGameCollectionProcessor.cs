using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Game = Domain.Entities.Game;
using CrossCutting;
using Domain.Broker;
using Domain.Repositories;
using Application.IgdbIntegrationOperations.SearchGame;
using System.Text.Json;
using Application.Processors.CollectionOperations.Shared;

namespace Application.Processors.CollectionOperations.ManageGameCollection;

public partial class UpdateGameCollectionProcessor : IRequestProcessor
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameCollectionItemRepository _userCollectionRepository;
    private readonly ISearchGame _searchGame;

    public UpdateGameCollectionProcessor (
        IGameRepository gameRepository, 
        IGameCollectionItemRepository userCollectionRepository, 
        ISearchGame searchGame
    )
    {
        _gameRepository = gameRepository;
        _userCollectionRepository = userCollectionRepository;
        _searchGame = searchGame;
    }
    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UpdateGameRequest>(field);
        var res = await UpdateGameAsync(request, cts);
    }
    

    private async Task<bool> UpdateGameAsync(UpdateGameRequest request, CancellationToken cts)
    {

        try
        {
            var foundGame = await _userCollectionRepository.SingleOrDefaultAsync(x => x.Id == request.Id, cts);

            if (! await _gameRepository.AnyAsync(g => g.GameId == foundGame.GameId, cts) && foundGame.GameId != 0)
            {
                var result = await _searchGame.RetrieveGameInfoAsync(foundGame.GameId);

                var gameInfo = result.Single();

                Game game = new()
                {
                    GameId = gameInfo.GameId,
                    Genres = gameInfo.Genres,
                    Description = gameInfo.Description,
                    Summary = gameInfo.Summary,
                    ImageUrl = gameInfo.Cover,
                    Title = gameInfo.Title,
                    ReleaseYear = gameInfo.FirstReleaseDate
                };

                await _gameRepository.AddAsync(game, cts);

            }

            var newGame = foundGame.MapAndFill<GameCollectionItem, UpdateGameRequest>(request);

            var res = await this._userCollectionRepository.UpdateAsync(newGame, cts);

            StdOut.Info("game updated");
            return true;
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DBConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidOperationException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (NullClaimException e)
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
