using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Infrastructure.Persistence;
using ProjectManagementService.Infrastructure.Repositories;
using ProjectManagementService.Infrastructure.Services;
using ProjectManagementService.Infrastructure.Messaging;

namespace ProjectManagementService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, string connectionString)
        {
            // Database
            services.AddDbContext<MyDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Repositories
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();

            // Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            
            // Kafka Producer
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            
            // Password Hasher
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // HttpContextAccessor (required for CurrentUserService)
            services.AddHttpContextAccessor();

            return services;
        }
    }
}