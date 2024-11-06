using Domain.Exceptions;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.UserCollectionOperations.ManageComputerCollection;

public class DeleteComputerCollectionProcessor : IRequestProcessor
{
    private readonly IUserComputerRepository _userComputerRepository;

    public DeleteComputerCollectionProcessor(IUserComputerRepository userComputerRepository)
    {
        _userComputerRepository = userComputerRepository;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserComputer>(field);
        var res = await DeleteComputerAsync(request, cts);
    }
    

    private async Task<bool> DeleteComputerAsync(UserComputer computer, CancellationToken cts)
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
