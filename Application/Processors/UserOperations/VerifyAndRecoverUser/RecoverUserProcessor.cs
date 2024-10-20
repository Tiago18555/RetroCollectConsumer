using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MailKit.Security;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using Domain;
using Domain.Repositories;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;

public partial class RecoverUserProcessor : IRequestProcessor
{
    private readonly IConfiguration _config;
    private readonly IRecoverRepository _recoverRepository;

    public RecoverUserProcessor(
        IConfiguration config, 
        IRecoverRepository recoverRepository
    )
    {
        _config = config;
        _recoverRepository = recoverRepository;
    }

    public async Task<MessageModel> CreateProcessAsync(string message, CancellationToken cts)
    {
        var field = message.ExtractMessage();
        var request = JsonSerializer.Deserialize<SendEmailInfo>(field);
        var res = await SendEmail(request, cts);

        return new MessageModel{ Message = res, SourceType = "recover-user" };
    }

    public async Task<ResponseModel> SendEmail(SendEmailInfo request, CancellationToken cts)
    {
        StdOut.Info("New message received...");
        string host = _config.GetSection("Host").Value;

        var resetInfo = new PasswordResetInfo
        {
            Username = request.Username,
            TimeStampHash = request.TimeStampHash,
            Success = false
        };        

        await _recoverRepository.InsertDocumentAsync("RecoverCollection", resetInfo.ToBsonDocument(), cts);

        var resetLink = $"{host}auth/recover/{request.Username}/{resetInfo.TimeStampHash}";

        var template = await File.ReadAllTextAsync(
            Path.Combine(
                System.Environment.CurrentDirectory,
                _config["BasePath"],
                "Static",
                "recover-template.html"
            )
        );

        var body = template
            .Replace("#resetLink", resetLink)
            .Replace("#userName", request.Username);
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:Username").Value));
            email.To.Add(MailboxAddress.Parse(request.Email));
            email.Subject = "RetroCollect Password Recover";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config.GetSection("Email:Host").Value, 587, SecureSocketOptions.StartTls, cts);
            await smtp.AuthenticateAsync(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value, cts);
            await smtp.SendAsync(email, cts);
            await smtp.DisconnectAsync(true, cts);

            StdOut.Info("Email sent");

            return "Email sent".Ok();
        }
        catch (Exception ex) {
            StdOut.Error($"ERROR: {ex.Message}");
            throw;
        }
    }
}
