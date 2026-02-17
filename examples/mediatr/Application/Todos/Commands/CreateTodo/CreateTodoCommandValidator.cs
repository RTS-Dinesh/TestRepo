using MediatrCodeSideExamples.Application.Common.Validation;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CreateTodo;

public sealed class CreateTodoCommandValidator : IRequestValidator<CreateTodoCommand>
{
    public Task<IReadOnlyCollection<string>> ValidateAsync(
        CreateTodoCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors.Add("Title is required.");
        }
        else if (request.Title.Length > 120)
        {
            errors.Add("Title must be 120 characters or fewer.");
        }

        return Task.FromResult<IReadOnlyCollection<string>>(errors);
    }
}

