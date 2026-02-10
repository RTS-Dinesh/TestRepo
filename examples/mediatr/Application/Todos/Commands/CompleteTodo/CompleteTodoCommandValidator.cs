using MediatrCodeSideExamples.Application.Common.Validation;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CompleteTodo;

public sealed class CompleteTodoCommandValidator : IRequestValidator<CompleteTodoCommand>
{
    public Task<IReadOnlyCollection<string>> ValidateAsync(
        CompleteTodoCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        if (request.Id == Guid.Empty)
        {
            errors.Add("Id is required.");
        }

        return Task.FromResult<IReadOnlyCollection<string>>(errors);
    }
}

