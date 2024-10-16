using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Configuration;
using Domain;
using Domain.Exceptions;
using Domain.Repositories;
using CrossCutting.Providers;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;

public partial class ChangePasswordProcessor : IRequestProcessor
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly IRecoverRepository _recoverRepository;
    private readonly IDateTimeProvider _timeProvider;

    public ChangePasswordProcessor(
        IConfiguration config, 
        IUserRepository userRepository, 
        IRecoverRepository recoverRepository, 
        IDateTimeProvider timeProvider
    )
    {
        _config = config;
        _userRepository = userRepository;
        _recoverRepository = recoverRepository;
        _timeProvider = timeProvider;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<ChangePasswordInfo>(field);

        var res = await ChangePassword(request);

        return new MessageModel{ Message = res, SourceType = "change-password" };
    }

    public async Task<ResponseModel> ChangePassword(ChangePasswordInfo request)
    {
        var foundUser = _userRepository.SingleOrDefault(u => u.UserId == request.userId);

        if (foundUser == null)            
            return ResponseFactory.NotFound("User Not Found");            

        if (!IsValidTimestampHash(request.timestampHash))            
            return "Password reset request has expired or is invalid".Ok();
        
        var hashedNewPassword = BCryptNet.HashPassword(request.password);

        if (BCryptNet.Verify(request.password, foundUser.Password))            
            return "New password cannot be equal to the old one".Ok();           

        foundUser.Password = hashedNewPassword;
        foundUser.UpdatedAt = _timeProvider.UtcNow;

        try
        {
            _userRepository.Update(foundUser);
            _recoverRepository.UpdateDocument("RecoverCollection", "UserId", foundUser.UserId, "Success", true);
            return "Password updated successfully".Ok();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Verifies if a time stamp hash is expired
    /// </summary>
    /// <param name="timestampHash"></param>
    /// <returns>true if time stamp hash is not expired</returns>
    private bool IsValidTimestampHash(string timestampHash)
    {
        int default_expires_at_value = 10;

        string expires_at = _config.GetSection("RecoverConfigs:PasswordResetTokenValidityMinutes").Value;

        if (string.IsNullOrEmpty(expires_at) || !int.TryParse(expires_at, out default_expires_at_value))
            throw new ConfigurationException("PasswordResetTokenValidityMinutes configuration is invalid");
        

        var timestampParts = timestampHash.Split('-');

        if (long.TryParse(timestampParts[0], out var requestTimestamp))
        {
            var currentTimestamp = _timeProvider.GetCurrentTimestampSeconds();
            long expirationTime = requestTimestamp + (int.Parse(expires_at) * 60);

            return currentTimestamp <= expirationTime;
        }

        return false;
    }

}
