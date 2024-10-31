using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Kafka.Consumers;

namespace Infrastructure.Kafka;

public class KafkaConsumerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public KafkaConsumerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CancellationToken getToken() =>_cts.Token;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>
        {
            Task.Run(async () =>
            {
                var service = _serviceProvider.GetRequiredService<CollectionConsumerService>();
                await service.ConsumeAsync(cancellationToken);
            }, cancellationToken),

            Task.Run(async () =>
            {
                var service = _serviceProvider.GetRequiredService<UserConsumerService>();
                await service.ConsumeAsync(cancellationToken);
            }, cancellationToken),

            Task.Run(async () =>
            {
                var service = _serviceProvider.GetRequiredService<RecoverConsumerService>();
                await service.ConsumeAsync(cancellationToken);
            }, cancellationToken),

            Task.Run(async () =>
            {
                var service = _serviceProvider.GetRequiredService<WishlistConsumerService>();
                await service.ConsumeAsync(cancellationToken);
            }, cancellationToken),

            Task.Run(async () =>
            {
                var service = _serviceProvider.GetRequiredService<RatingConsumerService>();
                await service.ConsumeAsync(cancellationToken);
            }, cancellationToken)
        };

        await Task.WhenAll(tasks);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
