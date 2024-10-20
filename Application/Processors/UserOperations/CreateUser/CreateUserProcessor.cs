using Domain.Broker;
using Domain.Entities;

using MimeKit;
using System.Data;
using MimeKit.Text;
using System.Text.Json;
using MailKit.Security;
using MailKit.Net.Smtp;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Configuration;
using Domain.Repositories;
using CrossCutting;

namespace Application.Processors.UserOperations.CreateUser;
public class CreateUserProcessor : IRequestProcessor
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _config;
    public CreateUserProcessor (
        IUserRepository repository, 
        IConfiguration config
    )
    {
        this._repository = repository;
        this._config = config;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<CreateUserRequest>(field);
        var res = await CreateUserAsync(request, cts);

        return new MessageModel{ Message = res, SourceType = "create-user" };
    }

    /// <exception cref="DBConcurrencyException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="System.Reflection.TargetException"></exception>
    /// <exception cref="System.Reflection.TargetInvocationException"></exception>
    /// <exception cref="MethodAccessException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="BCrypt.Net.SaltParseException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="DBUpdateException"></exception>
    public async Task<CreateUserResponseModel> CreateUserAsync(CreateUserRequest request, CancellationToken cts)
    {
        StdOut.Info("New message received...");
        try
        {
            User user = request.MapObjectTo(new User());

            user.Password = BCryptNet.HashPassword(request.Password);
            user.CreatedAt = DateTime.Now;

            var newUser = await this._repository.AddAsync(user, cts);

            await SendEmailToVerifyAsync(newUser);
            return newUser.MapObjectTo(new CreateUserResponseModel());
        }
        catch (Exception ex)
        {
            StdOut.Error($"ERROR: {ex.Message}");
            return new CreateUserResponseModel();
        }

    }


    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal async Task SendEmailToVerifyAsync(User user)
    {
        StdOut.Info("Sending email...");
        if (string.IsNullOrEmpty(user.Email))
        {
            throw new ArgumentException($"Valor de email não pode ser nulo. at {System.Environment.CurrentDirectory}");
        }
        string host = _config.GetSection("Host").Value;

        var verificationLink = $"{host}auth/verify/{user.Username}";

        var template = await File.ReadAllTextAsync(
            Path.Combine(
                _config["BasePath"],
                "Static",
                "verify-template.html"
            )
        );

        var body = template
            .Replace("#verificationLink", verificationLink)
            .Replace("#userName", user.Username);

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:Username").Value));
        email.To.Add(MailboxAddress.Parse(user.Email));
        email.Subject = "RetroCollect user verification";
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config.GetSection("Email:Host").Value, 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        StdOut.Info("Email sent");        
    }

}
