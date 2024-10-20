using Domain;
using Domain.Repositories;
using CrossCutting.Providers;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;
using Domain.Entities;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;

public partial class VerifyUserProcessor : IRequestProcessor
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _timeProvider;

    public VerifyUserProcessor(IUserRepository userRepository, IDateTimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _timeProvider = timeProvider;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<User>(field);

        var res = await VerifyUser(request, cts);

        return new MessageModel{ Message = res, SourceType = "verify-user" };
    }

    public async Task<ResponseModel> VerifyUser(User user, CancellationToken cts)
    {
        user.VerifiedAt = _timeProvider.UtcNow;

        var res = await _userRepository.UpdateAsync(user, cts);

        return res
            .MapObjectTo( new VerifyUserResponseModel() )
            .Ok();        
    }
}
