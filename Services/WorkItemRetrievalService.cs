using Spectre.Console;
using WorkItems.Models;

namespace WorkItems.Services;

public sealed class WorkItemRetrievalService
{
    private readonly AppConfig _config;
    private readonly AzureDevOpsClient _client;
    private readonly ReferenceParser _referenceParser;

    public WorkItemRetrievalService(
        AppConfig config,
        AzureDevOpsClient client,
        ReferenceParser referenceParser)
    {
        _config = config;
        _client = client;
        _referenceParser = referenceParser;
    }

    public async Task<RetrievalResult> RetrieveAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        ValidateConfig();

        var notes = new List<string>();
        var projectName = _config.DefaultProjectName!;
        var projectId = _config.DefaultProjectId!;
        var organization = _config.DefaultOrganization!;
        var pat = _config.Pat!;
        var baseUrl = _config.AzureDevOpsBaseUrl;

        var root = await _client.GetWorkItemAsync(baseUrl, organization, projectName, projectId, pat, workItemId, cancellationToken);
        root.Comments.AddRange(await _client.GetCommentsAsync(baseUrl, organization, projectName, pat, root.Id, cancellationToken));

        var parents = await LoadWorkItemsAsync(root.ParentIds, organization, projectName, projectId, pat, baseUrl, notes, cancellationToken);
        var children = await LoadWorkItemsAsync(root.ChildIds, organization, projectName, projectId, pat, baseUrl, notes, cancellationToken);

        var referenceSources = new[] { root }.Concat(parents).Concat(children).ToList();
        var parsedReferences = referenceSources
            .SelectMany(_referenceParser.ParseReferences)
            .Where(reference => reference.ReferencedWorkItemId != root.Id)
            .Where(reference => parents.All(parent => parent.Id != reference.ReferencedWorkItemId))
            .Where(reference => children.All(child => child.Id != reference.ReferencedWorkItemId))
            .ToList();

        var related = new List<RelatedWorkItem>();
        foreach (var referenceGroup in parsedReferences.GroupBy(reference => reference.ReferencedWorkItemId))
        {
            try
            {
                var relatedWorkItem = await _client.GetWorkItemAsync(
                    baseUrl,
                    organization,
                    projectName,
                    projectId,
                    pat,
                    referenceGroup.Key,
                    cancellationToken);
                relatedWorkItem.Comments.AddRange(await _client.GetCommentsAsync(baseUrl, organization, projectName, pat, relatedWorkItem.Id, cancellationToken));

                foreach (var reference in referenceGroup)
                {
                    reference.Retrieved = true;
                }

                related.Add(new RelatedWorkItem
                {
                    WorkItem = relatedWorkItem,
                    References = referenceGroup.ToList()
                });
            }
            catch (Exception ex)
            {
                foreach (var reference in referenceGroup)
                {
                    reference.Retrieved = false;
                }

                notes.Add($"Referenced work item {referenceGroup.Key} could not be loaded: {ex.Message}");
            }
        }

        return new RetrievalResult
        {
            RootWorkItem = root,
            Parents = parents,
            Children = children,
            RelatedWorkItems = related.OrderBy(item => item.WorkItem.Id).ToList(),
            Notes = notes
        };
    }

    private async Task<List<NormalizedWorkItem>> LoadWorkItemsAsync(
        IEnumerable<int> ids,
        string organization,
        string projectName,
        string projectId,
        string pat,
        string baseUrl,
        List<string> notes,
        CancellationToken cancellationToken)
    {
        var results = new List<NormalizedWorkItem>();
        foreach (var id in ids.Distinct().OrderBy(value => value))
        {
            try
            {
                var workItem = await _client.GetWorkItemAsync(baseUrl, organization, projectName, projectId, pat, id, cancellationToken);
                workItem.Comments.AddRange(await _client.GetCommentsAsync(baseUrl, organization, projectName, pat, workItem.Id, cancellationToken));
                results.Add(workItem);
            }
            catch (Exception ex)
            {
                notes.Add($"Work item {id} could not be loaded: {ex.Message}");
            }
        }

        return results;
    }

    private void ValidateConfig()
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(_config.Pat))
        {
            missing.Add("PAT");
        }

        if (string.IsNullOrWhiteSpace(_config.DefaultOrganization))
        {
            missing.Add("organization");
        }

        if (string.IsNullOrWhiteSpace(_config.DefaultProjectId) || string.IsNullOrWhiteSpace(_config.DefaultProjectName))
        {
            missing.Add("default project");
        }

        if (string.IsNullOrWhiteSpace(_config.OutputRootPath))
        {
            missing.Add("output path");
        }

        if (missing.Count > 0)
        {
            throw new InvalidOperationException($"Missing required configuration: {string.Join(", ", missing)}.");
        }
    }
}
