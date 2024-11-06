namespace Domain.Broker;
public interface IRequestProcessor
{
    void CreateProcessAsync(string message, CancellationToken cts);
}
