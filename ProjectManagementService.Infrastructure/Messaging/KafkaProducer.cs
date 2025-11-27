using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Messaging;

/// <summary>
/// Kafka Producer implementation
/// </summary>
public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            ClientId = configuration["Kafka:ClientId"] ?? "project-management-service",
            Acks = Acks.All,
            EnableIdempotence = true,
            MaxInFlight = 5,
            LingerMs = 10,
            CompressionType = CompressionType.Snappy
        };

        // Nếu có SASL authentication
        var saslUsername = configuration["Kafka:SaslUsername"];
        var saslPassword = configuration["Kafka:SaslPassword"];
        
        if (!string.IsNullOrEmpty(saslUsername) && !string.IsNullOrEmpty(saslPassword))
        {
            config.SecurityProtocol = SecurityProtocol.SaslSsl;
            config.SaslMechanism = SaslMechanism.ScramSha256;
            config.SaslUsername = saslUsername;
            config.SaslPassword = saslPassword;
        }

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = jsonMessage,
                Timestamp = Timestamp.Default
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            
            _logger.LogInformation(
                "Message published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                result.Topic, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Error publishing message to Kafka. Topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
