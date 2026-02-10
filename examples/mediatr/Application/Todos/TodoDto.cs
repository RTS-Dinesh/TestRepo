namespace MediatrCodeSideExamples.Application.Todos;

public sealed record TodoDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc);

