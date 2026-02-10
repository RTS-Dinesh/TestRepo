namespace MediatrCodeSideExamples.Application.Common.Validation;

public interface IRequestValidator<in TRequest>
{
    Task<IReadOnlyCollection<string>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken);
}

