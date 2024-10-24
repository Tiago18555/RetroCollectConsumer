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

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<ChangePasswordInfo>(field);

        var res = await ChangePasswordAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "change-password" };
    }

    public async Task<ResponseModel> ChangePasswordAsync(ChangePasswordInfo request, CancellationToken cts)
    {
        StdOut.Info("New message received...");
        var foundUser = await _userRepository.SingleOrDefaultAsync(u => u.Username == request.Username, cts);

        if (foundUser == null)            
            return ResponseFactory.NotFound("User Not Found");            

        if (!IsValidTimestampHash(request.TimestampHash))            
            return "Password reset request has expired or is invalid".Ok();
        
        var hashedNewPassword = BCryptNet.HashPassword(request.Password);

        if (BCryptNet.Verify(request.Password, foundUser.Password))            
            return "New password cannot be equal to the old one".Ok();           

        foundUser.Password = hashedNewPassword;
        foundUser.UpdatedAt = _timeProvider.UtcNow;

        try
        {
            await _userRepository.UpdateAsync(foundUser, cts);



            await _recoverRepository.UpdateDocumentAsync("RecoverCollection", "Username", foundUser.Username, "Success", true, cts);

            StdOut.Info("Password updated successfully");
            return "Password updated successfully".Ok();
        }
        catch (Exception ex)
        {
            StdOut.Error($"ERROR: {ex.Message}");
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
