using Domain;
using Domain.Exceptions;
using CrossCutting;
using Domain.Broker;
using Domain.Repositories;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.UserCollectionOperations.ManageGameCollection;

public partial class DeleteGameCollectionProcessor : IRequestProcessor
{
    private readonly IUserCollectionRepository _userCollectionRepository;

    public DeleteGameCollectionProcessor(IUserCollectionRepository userCollectionRepository)
    {
        _userCollectionRepository = userCollectionRepository;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserCollection>(field);
        var res = await DeleteGameAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "delete-game" };
    }

    public async Task<ResponseModel> DeleteGameAsync(UserCollection collection, CancellationToken cts)
    {
        try
        {
            if (await _userCollectionRepository.DeleteAsync(collection, cts))
            {
                return ResponseFactory.Ok("Game deleted");
            }
            else
            {
                return ResponseFactory.Ok("Not deleted");
            }
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch (NullClaimException msg)
        {
            return ResponseFactory.BadRequest(msg.ToString());
        }
    }
}
