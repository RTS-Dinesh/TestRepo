using MediatR;
using MediatrCodeSideExamples.Application.Todos;

namespace MediatrCodeSideExamples.Application.Todos.Queries.GetTodoById;

public sealed record GetTodoByIdQuery(Guid Id) : IRequest<TodoDto?>;

