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
        _logger.LogInformation("KafkaConsumerService ExecuteAsync started");
        
        // Chạy trong background task riêng để không block service startup
        _ = Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Starting Kafka consumer background task");
                await ConsumeKafkaMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Kafka consumer background task");
            }
        }, stoppingToken);

        _logger.LogInformation("KafkaConsumerService ExecuteAsync completed (background task launched)");
        await Task.CompletedTask;
    }

    private async Task ConsumeKafkaMessages(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConsumeKafkaMessages method started");
        
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];
        
        _logger.LogInformation("Kafka BootstrapServers: {BootstrapServers}", bootstrapServers ?? "(empty)");
        
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
            ClientId = _configuration["Kafka:ClientId"] ?? "utility-service",
            SocketTimeoutMs = 10000,
            SessionTimeoutMs = 10000
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
        
        _logger.LogInformation("Attempting to subscribe to topic: {Topic}", topic);
        _consumer.Subscribe(topic);

        _logger.LogInformation("✅ Kafka consumer started successfully. Listening to topic: {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Waiting for Kafka message...");
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("📩 Received Kafka message from topic {Topic}, Partition {Partition}, Offset {Offset}", 
                            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value);
                        
                        await ProcessMeetingCreatedEvent(consumeResult.Message.Value, stoppingToken);
                        _consumer.Commit(consumeResult);
                        
                        _logger.LogInformation("✅ Message processed and committed");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "❌ Error consuming message from Kafka");
                    await Task.Delay(5000, stoppingToken); // Wait before retry
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error processing Kafka message");
                }
            }
        }
        finally
        {
            _logger.LogInformation("Closing Kafka consumer");
            _consumer?.Close();
        }
    }

    private async Task ProcessMeetingCreatedEvent(string messageValue, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Processing meeting created event. Message: {Message}", messageValue);
        
        try
        {
            var meetingEvent = JsonSerializer.Deserialize<MeetingCreatedEvent>(messageValue);
            
            if (meetingEvent == null)
            {
                _logger.LogWarning("❌ Failed to deserialize meeting event");
                return;
            }

            _logger.LogInformation(
                "📅 Meeting Event Details - MeetingId: {MeetingId}, Title: {Title}, Attendees: {AttendeeCount}",
                meetingEvent.MeetingId, meetingEvent.Title, meetingEvent.Attendees.Count);

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            _logger.LogInformation("📧 Sending emails to {Count} attendees", meetingEvent.Attendees.Count);

            // Gửi email cho từng attendee
            foreach (var attendeeEmail in meetingEvent.Attendees)
            {
                try
                {
                    _logger.LogInformation("→ Attempting to send email to {Email}", attendeeEmail);
                    await emailService.SendMeetingInvitationAsync(meetingEvent, attendeeEmail);
                    _logger.LogInformation("✅ Successfully sent invitation email to {Email}", attendeeEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to send invitation email to {Email}. Error: {ErrorMessage}", 
                        attendeeEmail, ex.Message);
                }
            }
            
            _logger.LogInformation("✅ Completed processing meeting created event for MeetingId: {MeetingId}", meetingEvent.MeetingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing meeting created event. Error: {ErrorMessage}", ex.Message);
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
