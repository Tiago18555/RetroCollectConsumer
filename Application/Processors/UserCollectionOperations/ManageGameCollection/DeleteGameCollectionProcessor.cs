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

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserCollection>(field);
        var res = await DeleteGameAsync(request, cts);
    }

    private async Task<bool> DeleteGameAsync(UserCollection collection, CancellationToken cts)
    {
        try
        {
            if (await _userCollectionRepository.DeleteAsync(collection, cts))
            {
                StdOut.Error("Game deleted");
                return true;
            }
            else
            {
                StdOut.Error("Not deleted");
                return false;
            }
        }
        catch (ArgumentNullException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
        catch (NullClaimException e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
    }
}
