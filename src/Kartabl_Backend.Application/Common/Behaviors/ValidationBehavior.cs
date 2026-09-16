using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace Kartabl_Backend.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
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
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        // Map FluentValidation failures directly to Ardalis ValidationError instances
        var validationErrors = failures
            .Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage,
                ErrorCode = f.ErrorCode,
                Severity = (ValidationSeverity)(int)f.Severity
            })
            .ToList();

        // Dynamically construct Result<T>.Invalid(...) for generic responses
        if (typeof(TResponse).IsGenericType && 
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.Invalid), new[] { typeof(IEnumerable<ValidationError>) });

            return (TResponse)invalidMethod!.Invoke(null, new object[] { validationErrors })!;
        }

        // Handle non-generic Result return types
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Invalid(validationErrors);
        }

        return await next();
    }
}