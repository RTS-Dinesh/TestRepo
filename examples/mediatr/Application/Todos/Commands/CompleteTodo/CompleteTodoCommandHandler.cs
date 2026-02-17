using MediatR;
using MediatrCodeSideExamples.Application.Abstractions;
using MediatrCodeSideExamples.Application.Todos;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CompleteTodo;

public sealed class CompleteTodoCommandHandler : IRequestHandler<CompleteTodoCommand, TodoDto?>
{
    private readonly ITodoRepository _todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoDto?> Handle(
        CompleteTodoCommand request,
        CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (todo is null)
        {
            return null;
        }

        todo.Complete();
        await _todoRepository.UpdateAsync(todo, cancellationToken);
        return todo.ToDto();
    }
}

