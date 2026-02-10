using MediatR;
using MediatrCodeSideExamples.Application.Todos;

namespace MediatrCodeSideExamples.Application.Todos.Commands.CreateTodo;

public sealed record CreateTodoCommand(string Title) : IRequest<TodoDto>;

