using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectManagementService.Application.Behaviors;

// Tự động log tất cả request
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Log trước khi xử lý
        _logger.LogInformation("Bắt đầu xử lý: {RequestName}", requestName);

        var response = await next();

        // Log sau khi xử lý xong
        _logger.LogInformation("Hoàn thành: {RequestName}", requestName);

        return response;
    }
}