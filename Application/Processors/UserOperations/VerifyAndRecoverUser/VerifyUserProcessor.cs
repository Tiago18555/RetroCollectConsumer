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

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<User>(field);

        var res = await VerifyUser(request, cts);
    }

    private async Task<bool> VerifyUser(User user, CancellationToken cts)
    {
        user.VerifiedAt = _timeProvider.UtcNow;

        var res = await _userRepository.UpdateAsync(user, cts);

        return true;   
    }
}
