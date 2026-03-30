namespace WorkItems.Models;

public sealed class AdoProject
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Url { get; init; }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }
}
