namespace MediatrCodeSideExamples.Domain;

public sealed class TodoItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; private set; }

    public TodoItem(string title)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title is required.", nameof(title))
            : title.Trim();
    }

    public void Complete()
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;
        CompletedAtUtc = DateTime.UtcNow;
    }
}

