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
using Application.Processors.UserCollectionOperations.Shared;

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
        var request = JsonSerializer.Deserialize<UpdateGameRequest>(field);
        var res = await UpdateGameAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "update-game" };
    }
    

    public async Task<ResponseModel> UpdateGameAsync(UpdateGameRequest request, CancellationToken cts)
    {

        try
        {
            var foundGame = await _userCollectionRepository.SingleOrDefaultAsync(x => x.UserCollectionId == request.UserCollectionId, cts);

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

            var newGame = foundGame.MapAndFill<UserCollection, UpdateGameRequest>(request);

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
