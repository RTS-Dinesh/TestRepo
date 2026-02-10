using MediatR;
using MediatrCodeSideExamples.Application.Abstractions;
using MediatrCodeSideExamples.Application.Todos;

namespace MediatrCodeSideExamples.Application.Todos.Queries.GetTodoById;

public sealed class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDto?>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoDto?> Handle(
        GetTodoByIdQuery request,
        CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);
        return todo?.ToDto();
    }
}

