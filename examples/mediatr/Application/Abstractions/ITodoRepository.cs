using MediatrCodeSideExamples.Domain;

namespace MediatrCodeSideExamples.Application.Abstractions;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TodoItem item, CancellationToken cancellationToken);
    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken);
}

