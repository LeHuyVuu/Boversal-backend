using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementService.Application.Behaviors;
using ProjectManagementService.Application.Common;
using System.Reflection;

namespace ProjectManagementService.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // MediatR
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssemblies(assembly));

            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            // Mapster - Tự động scan và register tất cả IRegister implementations
            var config = TypeAdapterConfig.GlobalSettings;
            MappingConfig.ConfigureGlobalSettings(config);  // Apply global rules
            config.Scan(assembly);  // Tự động tìm và register ProjectMappingConfig, UserMappingConfig...
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}