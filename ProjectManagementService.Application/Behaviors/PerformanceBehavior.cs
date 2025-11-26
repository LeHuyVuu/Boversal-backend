using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ProjectManagementService.Application.Behaviors;

// Đo thời gian xử lý request, cảnh báo nếu chậm
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();

        var elapsed = _timer.ElapsedMilliseconds;

        // Nếu mất hơn 500ms thì cảnh báo
        if (elapsed > 500)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(
                "Request chạy chậm: {Name} ({Time}ms)",
                requestName,
                elapsed);
        }

        return response;
    }
}
