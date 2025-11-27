using System.Text.Json;
using Confluent.Kafka;
using UtilityService.Infrastructure;

namespace UtilityService.Messaging;

/// <summary>
/// Background service để consume Kafka messages
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private IConsumer<string, string>? _consumer;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];
        
        // Nếu chưa config Kafka, skip
        if (string.IsNullOrEmpty(bootstrapServers))
        {
            _logger.LogWarning("Kafka BootstrapServers not configured. Kafka consumer will not start.");
            return;
        }

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = _configuration["Kafka:GroupId"] ?? "utility-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            ClientId = _configuration["Kafka:ClientId"] ?? "utility-service"
        };

        // Nếu có SASL authentication
        var saslUsername = _configuration["Kafka:SaslUsername"];
        var saslPassword = _configuration["Kafka:SaslPassword"];
        
        if (!string.IsNullOrEmpty(saslUsername) && !string.IsNullOrEmpty(saslPassword))
        {
            config.SecurityProtocol = SecurityProtocol.SaslSsl;
            config.SaslMechanism = SaslMechanism.ScramSha256;
            config.SaslUsername = saslUsername;
            config.SaslPassword = saslPassword;
        }

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        
        var topic = _configuration["Kafka:MeetingCreatedTopic"] ?? "meeting-created";
        _consumer.Subscribe(topic);

        _logger.LogInformation("Kafka consumer started. Listening to topic: {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message?.Value != null)
                    {
                        await ProcessMeetingCreatedEvent(consumeResult.Message.Value, stoppingToken);
                        _consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message");
                }
            }
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMeetingCreatedEvent(string messageValue, CancellationToken cancellationToken)
    {
        try
        {
            var meetingEvent = JsonSerializer.Deserialize<MeetingCreatedEvent>(messageValue);
            
            if (meetingEvent == null)
            {
                _logger.LogWarning("Failed to deserialize meeting event");
                return;
            }

            _logger.LogInformation(
                "Processing meeting created event. MeetingId: {MeetingId}, Title: {Title}",
                meetingEvent.MeetingId, meetingEvent.Title);

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            // Gửi email cho từng attendee
            foreach (var attendeeEmail in meetingEvent.Attendees)
            {
                try
                {
                    await emailService.SendMeetingInvitationAsync(meetingEvent, attendeeEmail);
                    _logger.LogInformation("Sent invitation email to {Email}", attendeeEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send invitation email to {Email}", attendeeEmail);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing meeting created event");
            throw;
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}

/// <summary>
/// Event model cho meeting created
/// </summary>
public class MeetingCreatedEvent
{
    public long MeetingId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingLink { get; set; }
    public string OrganizerEmail { get; set; } = null!;
    public string OrganizerName { get; set; } = null!;
    public List<string> Attendees { get; set; } = new();
}
