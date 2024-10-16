﻿using CrossCutting;
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

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<UpdateUserRequest>(field);

        var res = await UpdateUser(request);

        return new MessageModel{ Message = res, SourceType = "update-user" };
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UpdateUserResponseModel> UpdateUser(UpdateUserRequest request)
    {
        User user = _repository.SingleOrDefault(x => x.UserId == request.UserId);

        var res = await this._repository.UpdateAsync(user.MapAndFill(request, _dateTimeProvider.UtcNow));

        return res
            .MapObjectTo( new UpdateUserResponseModel() );
    }
}