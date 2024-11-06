using Domain;
using Domain.Exceptions;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.UserCollectionOperations.ManageConsoleCollection;

public class DeleteConsoleCollectionProcessor : IRequestProcessor
{
    private readonly IUserConsoleRepository _userConsoleRepository;

    public DeleteConsoleCollectionProcessor(IUserConsoleRepository userConsoleRepository)
    {
        _userConsoleRepository = userConsoleRepository;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserConsole>(field);
        var res = await DeleteConsoleAsync(request, cts);
    }

    private async Task<bool> DeleteConsoleAsync(UserConsole console, CancellationToken cts)
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
