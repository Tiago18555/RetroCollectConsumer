using Domain.Broker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting;

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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var consumerService = scope.ServiceProvider.GetRequiredService<IConsumerService>();
                await consumerService.ConsumeAsync(_cts.Token);
            }
        }, _cts.Token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
