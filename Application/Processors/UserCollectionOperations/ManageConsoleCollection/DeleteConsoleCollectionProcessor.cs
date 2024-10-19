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

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserConsole>(field);
        var res = await DeleteConsoleAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "delete-console" };
    }

    public async Task<ResponseModel> DeleteConsoleAsync(UserConsole console, CancellationToken cts)
    {
        try
        {
            if (await _userConsoleRepository.DeleteAsync(console, cts))
            {
                return ResponseFactory.Ok("Console deleted");
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
