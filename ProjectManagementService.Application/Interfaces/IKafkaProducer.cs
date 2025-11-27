namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// Interface cho Kafka Producer service
/// </summary>
public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
}
