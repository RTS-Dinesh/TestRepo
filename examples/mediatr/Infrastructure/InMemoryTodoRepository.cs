using System.Collections.Concurrent;
using MediatrCodeSideExamples.Application.Abstractions;
using MediatrCodeSideExamples.Domain;

namespace MediatrCodeSideExamples.Infrastructure;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _storage = new();

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _storage.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _storage[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _storage[item.Id] = item;
        return Task.CompletedTask;
    }
}

