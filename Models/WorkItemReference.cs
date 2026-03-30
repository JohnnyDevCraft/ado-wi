namespace WorkItems.Models;

public sealed class WorkItemReference
{
    public int SourceWorkItemId { get; init; }

    public string SourceKind { get; init; } = string.Empty;

    public string SourceLabel { get; init; } = string.Empty;

    public string? SourceCommentId { get; init; }

    public int ReferencedWorkItemId { get; init; }

    public string ReferenceText { get; init; } = string.Empty;

    public bool Retrieved { get; set; }
}
