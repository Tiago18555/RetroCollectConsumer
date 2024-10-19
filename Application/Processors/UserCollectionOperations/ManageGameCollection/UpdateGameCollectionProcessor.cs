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

namespace Application.Processors.UserCollectionOperations.ManageGameCollection;

public partial class UpdateGameCollectionProcessor : IRequestProcessor
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserCollectionRepository _userCollectionRepository;
    private readonly ISearchGame _searchGame;

    public UpdateGameCollectionProcessor (
        IGameRepository gameRepository, 
        IUserCollectionRepository userCollectionRepository, 
        ISearchGame searchGame
    )
    {
        _gameRepository = gameRepository;
        _userCollectionRepository = userCollectionRepository;
        _searchGame = searchGame;
    }
    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserCollection>(field);
        var res = await UpdateGameAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "update-game" };
    }
    

    public async Task<ResponseModel> UpdateGameAsync(UserCollection newGame, CancellationToken cts)
    {

        try
        {
            if (! await _gameRepository.AnyAsync(g => g.GameId == newGame.GameId, cts) && newGame.GameId != 0)
            {
                var result = await _searchGame.RetrieveGameInfoAsync(newGame.GameId);

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

            var res = await this._userCollectionRepository.UpdateAsync(newGame, cts);

            return res.MapObjectsTo(new UpdateGameResponseModel()).Ok();
        }
        catch (ArgumentNullException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (DBConcurrencyException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (DbUpdateException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido");
        }
        catch (InvalidOperationException)
        {
            throw;
            //return GenericResponses.NotAcceptable("Formato de dados inválido.");
        }
        catch (NullClaimException msg)
        {
            return ResponseFactory.BadRequest(msg.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }
}
