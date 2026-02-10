using MediatR;
using MediatrCodeSideExamples.Application.Common.Validation;
using System.Linq;

namespace MediatrCodeSideExamples.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IRequestValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IRequestValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var errorLists = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        var errors = errorLists.SelectMany(x => x).Distinct().ToArray();
        if (errors.Length > 0)
        {
            throw new RequestValidationException(errors);
        }

        return await next();
    }
}

