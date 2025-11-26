using FluentValidation;
using MediatR;
using ValidationException = ProjectManagementService.Domain.Exceptions.ValidationException;

namespace ProjectManagementService.Application.Behaviors;

// Tự động validate tất cả request trước khi xử lý
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Nếu không có validator thì bỏ qua
        if (!_validators.Any())
        {
            return await next();
        }

        // Chạy tất cả validators
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Lấy tất cả lỗi
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Nếu có lỗi thì throw exception
        if (failures.Any())
        {
            var errors = failures
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            throw new ValidationException(errors);
        }

        return await next();
    }
}
