using MediatR;
using MediatrCodeSideExamples.Application.Abstractions;
using MediatrCodeSideExamples.Application.Todos;
using MediatrCodeSideExamples.Domain;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CreateTodo;

public sealed class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem(request.Title);
        await _todoRepository.AddAsync(todo, cancellationToken);
        return todo.ToDto();
    }
}

