# .NET MediatR Architecture - Code-Side Examples

This folder contains practical, code-first examples of a layered MediatR setup in .NET:

- **Domain**: business entities only
- **Application**: commands, queries, handlers, behaviors
- **Infrastructure**: data access implementation
- **Api**: endpoint wiring and MediatR dispatch

## Typical package setup

In your real project, add MediatR packages with:

```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

## Architecture flow

1. API receives HTTP request.
2. API sends a command/query via `IMediator`.
3. Pipeline behaviors run (for example validation, logging).
4. Handler executes application logic through abstractions (for example repository interfaces).
5. Infrastructure provides concrete implementations.

## Included examples

- `CreateTodoCommand` + handler
- `CompleteTodoCommand` + handler
- `GetTodoByIdQuery` + handler
- `ValidationBehavior` and `RequestLoggingBehavior`
- `InMemoryTodoRepository`
- Minimal API endpoint mapping with MediatR

