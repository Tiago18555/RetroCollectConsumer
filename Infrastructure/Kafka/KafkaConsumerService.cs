using System.Text.Json;

using Confluent.Kafka;
using Domain.Broker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CrossCutting;

namespace Infrastructure.Kafka;
public partial class KafkaConsumerService: IConsumerService
{
    private readonly IRequestProcessorFactory _processorFactory;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IConfiguration _configuration;
    private readonly Parameters _parameters;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger, 
        IRequestProcessorFactory processorFactory,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _processorFactory = processorFactory;
        _parameters = new Parameters();
        _configuration = configuration;

        _parameters.BootstrapServer = _configuration
            .GetSection("KafkaConfig")
            .GetSection("BootstrapServer").Value;

        _consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = _parameters.BootstrapServer,
            GroupId = _parameters.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
    }


    private string GetMessageType(string message)
    {  
        var _object = JsonSerializer.Deserialize(message, typeof(MessageModel)) as MessageModel;
        return _object.SourceType;
    }

    public async Task ConsumeAsync(CancellationToken cts)
    {
        _logger.LogInformation("Waiting messages");
        await CreateTopicIfNotExistsAsync(_parameters.TopicName);
        _consumer.Subscribe(_parameters.TopicName);

        while (!cts.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cts);
                var messageType = GetMessageType(result.Message.Value);

                var processor = _processorFactory.Create(messageType); //GET PROCESSOR TYPE

                var processResult = await processor.CreateProcessAsync(result.Message.Value, cts); //CALL

                var data = JsonSerializer.Serialize(result.Message.Value);
                _logger.LogInformation("GroupId: {@GroupId} Message: {Message}", _parameters, processResult.Message);
                _logger.LogInformation(data.ToString());
            }
            catch(ConsumeException err)
            {
                _logger.LogError(err, "There's no topic of this message.");
                StdOut.Error($"Consume error: \n{err.Message}");
            }
            catch(DbUpdateException err)
            {
                _logger.LogInformation($"An error occurs on the operation: {err.Message}");
                StdOut.Error($"DbUpdate error: \n{err.Message}");
            }
            finally
            {
                await StopAsync(cts);
            }
        }

        return;
    }

    public Task StopAsync(CancellationToken cts)
    {
        _consumer.Close();
        StdOut.Error("Application stopped. Connection closed");
        _logger.LogInformation("Application stopped. Connection closed");
        return Task.CompletedTask;
    }

    public Task RetryAsync()
    {
        throw new NotImplementedException();
    }

}
