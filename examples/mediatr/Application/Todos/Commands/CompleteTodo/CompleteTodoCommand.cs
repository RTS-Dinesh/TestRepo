using MediatR;
using MediatrCodeSideExamples.Application.Todos;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CompleteTodo;

public sealed record CompleteTodoCommand(Guid Id) : IRequest<TodoDto?>;

