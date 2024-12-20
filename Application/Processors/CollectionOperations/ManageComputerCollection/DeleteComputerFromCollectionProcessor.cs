using Domain.Exceptions;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.CollectionOperations.ManageComputerCollection;

public class DeleteComputerFromCollectionProcessor : IRequestProcessor
{
    private readonly IComputerCollectionItemRepository _userComputerRepository;

    public DeleteComputerFromCollectionProcessor(IComputerCollectionItemRepository userComputerRepository)
    {
        _userComputerRepository = userComputerRepository;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<ComputerCollectionItem>(field);
        var res = await DeleteComputerAsync(request, cts);
    }
    

    private async Task<bool> DeleteComputerAsync(ComputerCollectionItem computer, CancellationToken cts)
    {
        try
        {
            if (await _userComputerRepository.DeleteAsync(computer, cts))
            {
                StdOut.Info("computer deleted");
                return true;
            }
            else
            {
                StdOut.Error("not deleted");
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
