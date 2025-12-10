using System.Text.Json;
using MediatR;
using ProjectManagementService.Application.DTOs.Meeting;
using ProjectManagementService.Application.Events;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagementService.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// Handler xử lý CreateMeetingCommand
/// </summary>
public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUserRepository _userRepository;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public CreateMeetingCommandHandler(
        IMeetingRepository meetingRepository,
        ICurrentUserService currentUserService,
        IKafkaProducer kafkaProducer,
        IUserRepository userRepository,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _meetingRepository = meetingRepository;
        _currentUserService = currentUserService;
        _kafkaProducer = kafkaProducer;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<MeetingDto> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");

        // Tạo Meeting entity
        var meeting = new Meeting
        {
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MeetingLink = request.MeetingLink,
            UserId = userId,
            Attendees = JsonSerializer.Serialize(request.Attendees)
        };

        // Lưu vào DB
        var createdMeeting = await _meetingRepository.CreateAsync(meeting);

        // Lấy thông tin user để gửi email
        var user = await _userRepository.GetByIdAsync(userId);
        
        // Publish Kafka event để gửi email
        var kafkaBootstrap = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
        if (!string.IsNullOrEmpty(kafkaBootstrap))
        {
            var topic = "meeting-created";
            await _kafkaProducer.PublishAsync(topic, new MeetingCreatedEvent
            {
                MeetingId = createdMeeting.Id,
                Title = createdMeeting.Title,
                Description = createdMeeting.Description,
                StartTime = createdMeeting.StartTime,
                EndTime = createdMeeting.EndTime,
                MeetingLink = createdMeeting.MeetingLink,
                OrganizerEmail = user?.Email ?? "unknown@example.com",
                OrganizerName = user?.FullName ?? user?.Username ?? "Unknown",
                Attendees = request.Attendees
            }, cancellationToken);
        }

        // Map to DTO
        return new MeetingDto
        {
            Id = createdMeeting.Id,
            Title = createdMeeting.Title,
            Description = createdMeeting.Description,
            StartTime = createdMeeting.StartTime,
            EndTime = createdMeeting.EndTime,
            MeetingLink = createdMeeting.MeetingLink,
            UserId = createdMeeting.UserId,
            Attendees = JsonSerializer.Deserialize<List<string>>(createdMeeting.Attendees) ?? new List<string>(),
            CreatedAt = createdMeeting.CreatedAt,
            UpdatedAt = createdMeeting.UpdatedAt
        };
    }
}
