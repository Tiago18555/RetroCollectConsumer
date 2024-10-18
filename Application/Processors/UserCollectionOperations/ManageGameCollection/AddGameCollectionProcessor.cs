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

namespace Application.Processors.UserCollectionOperations.ManageGameCollection;

public partial class AddGameCollectionProcessor : IRequestProcessor
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserCollectionRepository _userCollectionRepository;
    private readonly ISearchGame _searchGame;
    public AddGameCollectionProcessor(
        IGameRepository gameRepository,
        IUserCollectionRepository userCollectionRepository,
        ISearchGame searchGame
    )
    {
        this._gameRepository = gameRepository;
        this._userCollectionRepository = userCollectionRepository;
        this._searchGame = searchGame;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<AddGameRequestModel>(field);
        var res = await AddGameAsync(request);

        return new MessageModel{ Message = res, SourceType = "add-game" };
    }

    public async Task<ResponseModel> AddGameAsync(AddGameRequestModel requestBody)
    {

        if (!_gameRepository.Any(g => g.GameId == requestBody.Game_id))
        {
            try
            {
                var result = await _searchGame.RetrieveGameInfoAsync(requestBody.Game_id);

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


                _gameRepository.Add(game);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        try
        {
            UserCollection userCollection = new()
            {
                ConsoleId = requestBody.PlatformIsComputer == false ? requestBody.Platform_id : 0,
                ComputerId = requestBody.PlatformIsComputer == true ? requestBody.Platform_id : 0,
                GameId = requestBody.Game_id,
                UserId = requestBody.User_id,
                Condition = Enum.Parse<Condition>(requestBody.Condition.ToCapitalize(typeof(Condition))),
                OwnershipStatus = Enum.Parse<OwnershipStatus>(requestBody.OwnershipStatus.ToCapitalize(typeof(OwnershipStatus))),
                Notes = requestBody.Notes == null ? null : requestBody.Notes,
                PurchaseDate = requestBody.PurchaseDate == DateTime.MinValue ? DateTime.MinValue : requestBody.PurchaseDate
            };

            var res = _userCollectionRepository.Add(userCollection);
            return res.MapObjectsTo(new AddGameResponseModel()).Created();
        }
        catch (NullClaimException msg)
        {
            return ResponseFactory.BadRequest(msg.ToString());
        }
        catch (DBConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (InvalidEnumTypeException msg)
        {
            return ResponseFactory.UnsupportedMediaType("Invalid type for Condition or OwnershipStatus: " + msg);
        }
        catch (InvalidEnumValueException msg)
        {
            return ResponseFactory.BadRequest("Invalid value for Condition or OwnershipStatus: " + msg);
        }
        
    }
}
