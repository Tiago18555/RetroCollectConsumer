using Confluent.Kafka;
using Confluent.Kafka.Admin;
using CrossCutting;

namespace Infrastructure.Kafka;

public static class KafkaConsumerStartUp
{
    internal static async Task CreateIfNotExistsAsync(this string topicName, string bootstrapServer)
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = bootstrapServer
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
                StdOut.Info($"Tópico '{topicName}' criado com sucesso.");
            }
        }
        catch (KafkaException ex)
        {
            StdOut.Error($"Erro ao verificar ou criar tópico: {ex.Message}");
        }
    }

}