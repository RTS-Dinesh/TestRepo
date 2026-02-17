using MediatrCodeSideExamples.Application.Common.Validation;

namespace MediatrCodeSideExamples.Application.Todos.Queries.GetTodoById;

public sealed class GetTodoByIdQueryValidator : IRequestValidator<GetTodoByIdQuery>
{
    public Task<IReadOnlyCollection<string>> ValidateAsync(
        GetTodoByIdQuery request,
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

