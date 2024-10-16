using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MailKit.Security;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using Domain;
using Domain.Repositories;
using CrossCutting.Providers;
using CrossCutting;
using Domain.Broker;
using System.Text.Json;

namespace Application.Processors.UserOperations.VerifyAndRecoverUser;

public partial class VerifyAndRecoverUserProcessor : IRequestProcessor
{
    private readonly IConfiguration _config;
    private readonly IRecoverRepository _recoverRepository;
    private readonly IDateTimeProvider _timeProvider;

    public VerifyAndRecoverUserProcessor(
        IConfiguration config, 
        IRecoverRepository recoverRepository, 
        IDateTimeProvider timeProvider
    )
    {
        _config = config;
        _recoverRepository = recoverRepository;
        _timeProvider = timeProvider;
    }

    public async Task<MessageModel> CreateProcessAsync(string message)
    {
        var field = message.ExtractMessage();

        System.Console.WriteLine("STRING MESSAGE");
        System.Console.WriteLine(field);

        var request = JsonSerializer.Deserialize<SendEmailInfo>(field);

        var res = await SendEmail(request);

        return new MessageModel{ Message = res, SourceType = "verify-recover-user" };
    }

    public async Task<ResponseModel> SendEmail(SendEmailInfo request)
    {
        string host = _config.GetSection("Host").Value;

        var resetInfo = new PasswordResetInfo
        {
            UserId = request.UserId,
            Hash = request.TimeStampHash,
            Timestamp = request.TimeStampHash,
            Success = false
        };        

        _recoverRepository.InsertDocument("RecoverCollection", resetInfo.ToBsonDocument());

        var resetLink = $"{host}auth/recover/{request.UserId}/{resetInfo.Timestamp}";


        var template = File.ReadAllText(

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

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine(body);
        System.Console.ForegroundColor = ConsoleColor.White;

        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:Username").Value));
            email.To.Add(MailboxAddress.Parse(request.Email));
            email.Subject = "RetroCollect Password Recover";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("Email:Host").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);

            return "Email sent".Ok();
        }
        catch (Exception msg) { return ResponseFactory.ServiceUnavailable(msg.ToString()); }
    }

    #region inner methods
}

#endregion
