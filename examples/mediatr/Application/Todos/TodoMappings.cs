using MediatrCodeSideExamples.Domain;

namespace MediatrCodeSideExamples.Application.Todos;

public static class TodoMappings
{
    public static TodoDto ToDto(this TodoItem item) =>
        new(
            item.Id,
            item.Title,
            item.IsCompleted,
            item.CreatedAtUtc,
            item.CompletedAtUtc);
}

