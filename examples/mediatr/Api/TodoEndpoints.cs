using MediatR;
using MediatrCodeSideExamples.Application.Todos.Commands.CompleteTodo;
using MediatrCodeSideExamples.Application.Todos.Commands.CreateTodo;
using MediatrCodeSideExamples.Application.Todos.Queries.GetTodoById;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MediatrCodeSideExamples.Api;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos");

        group.MapPost(
            "/",
            async (CreateTodoCommand command, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var created = await mediator.Send(command, cancellationToken);
                return Results.Created($"/todos/{created.Id}", created);
            });

        group.MapPut(
            "/{id:guid}/complete",
            async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var completed = await mediator.Send(new CompleteTodoCommand(id), cancellationToken);
                return completed is null ? Results.NotFound() : Results.Ok(completed);
            });

        group.MapGet(
            "/{id:guid}",
            async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var todo = await mediator.Send(new GetTodoByIdQuery(id), cancellationToken);
                return todo is null ? Results.NotFound() : Results.Ok(todo);
            });

        return app;
    }
}

