namespace WorkItems.Models;

public sealed class AppConfig
{
    public string StorageRootPath { get; set; } = string.Empty;

    public string OutputRootPath { get; set; } = string.Empty;

    public string AzureDevOpsBaseUrl { get; set; } = "https://dev.azure.com";

    public string? DefaultOrganization { get; set; }

    public string? DefaultProjectId { get; set; }

    public string? DefaultProjectName { get; set; }

    public string AuthMode { get; set; } = "pat";

    public string? Pat { get; set; }

    public string ConsoleUiProvider { get; set; } = "Spectre.Console";

    public string? LastExportPath { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
