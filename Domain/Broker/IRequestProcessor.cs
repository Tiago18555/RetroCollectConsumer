namespace Domain.Broker;
public interface IRequestProcessor
{
    Task<MessageModel> CreateProcessAsync(string message);
}
