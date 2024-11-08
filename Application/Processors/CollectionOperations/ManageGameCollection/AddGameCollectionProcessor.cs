using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Game = Domain.Entities.Game;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using Application.IgdbIntegrationOperations.SearchGame;
using System.Text.Json;

namespace Application.Processors.CollectionOperations.ManageGameCollection;

public partial class AddGameCollectionProcessor : IRequestProcessor
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameCollectionItemRepository _userCollectionRepository;
    private readonly ISearchGame _searchGame;
    public AddGameCollectionProcessor(
        IGameRepository gameRepository,
        IGameCollectionItemRepository userCollectionRepository,
        ISearchGame searchGame
    )
    {
        this._gameRepository = gameRepository;
        this._userCollectionRepository = userCollectionRepository;
        this._searchGame = searchGame;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddGameRequest>(field);
        var res = await AddGameAsync(request, cts);
    }

    private async Task<bool> AddGameAsync(AddGameRequest requestBody, CancellationToken cts)
    {

        if (! await _gameRepository.AnyAsync(g => g.GameId == requestBody.GameId, cts))
        {
            try
            {
                var result = await _searchGame.RetrieveGameInfoAsync(requestBody.GameId);

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
        }

        try
        {
            GameCollectionItem userCollection = new()
            {
                ConsoleId = requestBody.PlatformIsComputer == false ? requestBody.PlatformId : 0,
                ComputerId = requestBody.PlatformIsComputer == true ? requestBody.PlatformId : 0,
                GameId = requestBody.GameId,
                UserId = requestBody.UserId,

                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = await _userCollectionRepository.AddAsync(userCollection, cts);
            StdOut.Info("game added to collection");
            return true;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DBConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (DbUpdateConcurrencyException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidEnumTypeException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (InvalidEnumValueException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        
    }
}
