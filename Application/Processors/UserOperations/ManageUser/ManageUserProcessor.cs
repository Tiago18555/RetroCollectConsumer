using CrossCutting;
using CrossCutting.Providers;
using Domain.Broker;
using Domain.Entities;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Processors.UserOperations.ManageUser;

public class ManageUserProcessor : IRequestProcessor
{
    private readonly IUserRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    public ManageUserProcessor
    (
        IUserRepository repository, 
        IDateTimeProvider dateTimeProvider
    )
    {
        this._repository = repository;
        this._dateTimeProvider = dateTimeProvider;
    }

    public async void CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UpdateUserRequest>(field);

        StdOut.Error(field);

        var res = await UpdateUser(request, cts);
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<bool> UpdateUser(UpdateUserRequest request, CancellationToken cts)
    {
        try
        {
            StdOut.Info("New message received...");
            User user = await _repository.SingleOrDefaultAsync(x => x.UserId == request.UserId, cts);

            var res = await this._repository.UpdateAsync(user.MapAndFill(request, _dateTimeProvider.UtcNow), cts);

            StdOut.Info("User updated successfully");
            return true;
        }
        catch(Exception e)
        {
            StdOut.Error($"ERROR: {e.Message}");
            return false;
        }
    }
}
