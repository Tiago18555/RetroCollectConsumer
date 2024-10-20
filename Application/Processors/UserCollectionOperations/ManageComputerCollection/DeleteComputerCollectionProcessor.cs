using Domain;
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

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UserComputer>(field);
        var res = await DeleteComputerAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "delete-computer" };
    }
    

    public async Task<ResponseModel> DeleteComputerAsync(UserComputer computer, CancellationToken cts)
    {
        try
        {
            if (await _userComputerRepository.DeleteAsync(computer, cts))
            {
                return ResponseFactory.Ok("Computer deleted");
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
