using System.Text.RegularExpressions;

namespace WorkItems.Services;

public sealed class AppPaths
{
    public string GetStorageRoot()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".ADO-WI");
    }

    public string GetConfigPath()
    {
        return Path.Combine(GetStorageRoot(), "ADO-WI.config");
    }

    public string GetSnapshotRoot()
    {
        return Path.Combine(GetStorageRoot(), "workitems");
    }

    public string GetCommentsRoot()
    {
        return Path.Combine(GetStorageRoot(), "comments");
    }

    public string GetProjectCachePath()
    {
        return Path.Combine(GetStorageRoot(), "projects.json");
    }

    public string GetExportPath(string outputRoot, string projectName, int workItemId, string title)
    {
        Directory.CreateDirectory(outputRoot);
        var slug = Slugify(title);
        return Path.Combine(outputRoot, $"{Slugify(projectName)}-{workItemId}-{slug}.md");
    }

    public static string Slugify(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "work-item";
        }

        var normalized = value.Trim().ToLowerInvariant();
        normalized = Regex.Replace(normalized, @"[^a-z0-9]+", "-");
        normalized = Regex.Replace(normalized, @"-+", "-");
        return normalized.Trim('-');
    }
}
