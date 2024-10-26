using Confluent.Kafka;
using Confluent.Kafka.Admin;
using CrossCutting;
using Domain.Broker;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka;

public partial class KafkaConsumerService: IConsumerService
{
    private async Task CreateTopicIfNotExistsAsync(string topicName)
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = _parameters.BootstrapServer
        };

        using var adminClient = new AdminClientBuilder(config).Build();
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            var topicExists = metadata.Topics.Any(t => t.Topic == topicName);

            if (!topicExists)
            {
                await adminClient.CreateTopicsAsync(new[]
                {
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = 1,
                            ReplicationFactor = 1
                        }
                    });
                _logger.LogInformation($"Tópico '{topicName}' criado com sucesso.");
                StdOut.Info($"Tópico '{topicName}' criado com sucesso.");
            }
        }
        catch (KafkaException ex)
        {
            _logger.LogError($"Erro ao verificar ou criar tópico: {ex.Message}");
            StdOut.Error($"Erro ao verificar ou criar tópico: {ex.Message}");
        }
    }

}