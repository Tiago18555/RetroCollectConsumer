using Domain.Exceptions;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.CollectionOperations.ManageConsoleCollection;

public class DeleteConsoleCollectionProcessor : IRequestProcessor
{
    private readonly IConsoleCollectionItemRepository _userConsoleRepository;

    public DeleteConsoleCollectionProcessor(IConsoleCollectionItemRepository userConsoleRepository)
    {
        _userConsoleRepository = userConsoleRepository;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<ConsoleCollectionItem>(field);
        var res = await DeleteConsoleAsync(request, cts);
    }

    private async Task<bool> DeleteConsoleAsync(ConsoleCollectionItem console, CancellationToken cts)
    {
        try
        {
            if (await _userConsoleRepository.DeleteAsync(console, cts))
            {
                StdOut.Info($"console deleted");
                return true;
            }
            else
            {
                StdOut.Error($"not deleted");
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
