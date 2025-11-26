// using System.Text.Json;
// using Confluent.Kafka;
// using Microsoft.Extensions.DependencyInjection;
// using UtilityService.Infrastructure.Services;
// using UtilityService.Models;
//
// namespace UtilityService.Messaging;
//
// public class TestDriveCreatedConsumer : BackgroundService
// {
//     private readonly IConfiguration _cfg;
//     private readonly ILogger<TestDriveCreatedConsumer> _logger;
//     private readonly IServiceScopeFactory _scopeFactory;
//
//     private static readonly JsonSerializerOptions _jsonOpts = new()
//     {
//         PropertyNameCaseInsensitive = true
//     };
//
//     public TestDriveCreatedConsumer(
//         IConfiguration cfg,
//         ILogger<TestDriveCreatedConsumer> logger,
//         IServiceScopeFactory scopeFactory)
//     {
//         _cfg = cfg;
//         _logger = logger;
//         _scopeFactory = scopeFactory;
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // Nhả control ngay để host tiếp tục khởi động
//         await Task.Yield();
//
//         var bootstrap = _cfg["KafkaSettings:BootstrapServers"];
//         var topic = _cfg["KafkaSettings:Topics:Dealer"];
//
//         if (string.IsNullOrWhiteSpace(bootstrap) || string.IsNullOrWhiteSpace(topic))
//         {
//             _logger.LogWarning("Kafka config missing. BootstrapServers='{Bootstrap}', Topic='{Topic}'. Consumer will idle.", bootstrap, topic);
//             // Idle loop: không block host, chờ cấu hình/hạ tầng sẵn sàng
//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//             }
//             return;
//         }
//
//         var consumerCfg = new ConsumerConfig
//         {
//             BootstrapServers = bootstrap,
//             GroupId = "utility-service-consumer",
//             AutoOffsetReset = AutoOffsetReset.Earliest,
//             EnableAutoCommit = false,
//             // làm consumer responsive hơn khi hạ tầng trục trặc
//             SocketTimeoutMs = 10_000,
//             SessionTimeoutMs = 10_000,
//             MaxPollIntervalMs = 300_000,
//             EnablePartitionEof = true
//         };
//
//         using var consumer = new ConsumerBuilder<Ignore, string>(consumerCfg)
//             .SetErrorHandler((_, e) =>
//             {
//                 // lỗi “broker down” không nên giết app; chỉ log ở mức Warning
//                 if (e.IsFatal)
//                     _logger.LogError("Kafka fatal error: {Reason}", e.Reason);
//                 else
//                     _logger.LogWarning("Kafka error: {Reason}", e.Reason);
//             })
//             .Build();
//
//         _logger.LogInformation("Kafka consumer starting. Bootstrap='{Bootstrap}', Topic='{Topic}', Group='{Group}'",
//             bootstrap, topic, consumerCfg.GroupId);
//
//         try
//         {
//             consumer.Subscribe(topic);
//             _logger.LogInformation("Kafka consumer subscribed to topic: {Topic}", topic);
//
//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 try
//                 {
//                     // Dùng timeout để vòng lặp có cơ hội kiểm tra cancellation và không block host
//                     var cr = consumer.Consume(TimeSpan.FromSeconds(1));
//                     if (cr is null) continue;              // timeout poll
//                     if (cr.IsPartitionEOF) continue;       // chạm EOF, bỏ qua
//
//                     // Log thô có thể noisy; giữ mức Information
//                     _logger.LogInformation("Kafka message @ {Topic}[{Partition}]#{Offset}",
//                         cr.Topic, cr.Partition, cr.Offset);
//
//                     TestDriveCreatedEvent? ev = null;
//                     try
//                     {
//                         ev = JsonSerializer.Deserialize<TestDriveCreatedEvent>(cr.Message.Value, _jsonOpts);
//                     }
//                     catch (Exception ex)
//                     {
//                         _logger.LogWarning(ex, "Cannot deserialize payload: {Payload}", cr.Message.Value);
//                     }
//
//                     if (ev is null)
//                     {
//                         _logger.LogWarning("Message skipped: payload null/invalid");
//                     }
//                     else
//                     {
//                         _logger.LogInformation("Event parsed: TestDriveId={Id}, Dealer={Dealer}, ConfirmEmail={Confirm}, Email={Email}",
//                             ev.TestDriveId, ev.DealerId, ev.ConfirmEmail, ev.Email);
//
//                         if (ev.ConfirmEmail && !string.IsNullOrWhiteSpace(ev.Email))
//                         {
//                             using var scope = _scopeFactory.CreateScope();
//                             var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
//
//                             var emailDto = new EmailRequestDto
//                             {
//                                 ToEmail = ev.Email,
//                                 Subject = $"Xác nhận lịch lái thử #{ev.TestDriveId}",
//                                 Content = $"Bạn đã đặt lịch lái thử ngày {ev.DriveDate:dd/MM/yyyy} - {ev.TimeSlot}"
//                             };
//
//                             try
//                             {
//                                 _logger.LogInformation("Sending email to {To}...", emailDto.ToEmail);
//                                 var result = await emailService.SendEmailAsync(emailDto);
//                                 _logger.LogInformation("Send email result: status={Status}, message={Message}", result.Status, result.Message);
//                             }
//                             catch (Exception ex)
//                             {
//                                 _logger.LogError(ex, "Send email failed to {To}", emailDto.ToEmail);
//                             }
//                         }
//                     }
//
//                     try
//                     {
//                         consumer.Commit(cr);
//                     }
//                     catch (Exception exCommit)
//                     {
//                         _logger.LogWarning(exCommit, "Commit failed at {PartitionOffset}", cr.TopicPartitionOffset);
//                     }
//                 }
//                 catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
//                 {
//                     // normal shutdown
//                     break;
//                 }
//                 catch (ConsumeException cex)
//                 {
//                     // lỗi consume riêng: log và tiếp tục
//                     _logger.LogWarning(cex, "Consume error: {Reason}", cex.Error.Reason);
//                     await Task.Delay(500, stoppingToken);
//                 }
//                 catch (Exception ex)
//                 {
//                     // các lỗi khác: log và backoff ngắn
//                     _logger.LogError(ex, "Unhandled error in consumer loop");
//                     await Task.Delay(1000, stoppingToken);
//                 }
//             }
//         }
//         finally
//         {
//             try
//             {
//                 consumer.Close();
//             }
//             catch { /* ignore */ }
//
//             _logger.LogInformation("Kafka consumer closed.");
//         }
//     }
// }
//
// public record TestDriveCreatedEvent(
//     Guid TestDriveId,
//     Guid DealerId,
//     Guid CustomerId,
//     Guid VehicleVersionId,
//     DateTime DriveDate,
//     string TimeSlot,
//     bool ConfirmSms,
//     string Email,
//     bool ConfirmEmail,
//     string Status,
//     DateTime OccurredAtUtc
// );
