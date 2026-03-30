using Spectre.Console;
using WorkItems.ConsoleUi;
using WorkItems.Models;
using WorkItems.Services;

namespace WorkItems.Workflows;

public sealed class Application
{
    private readonly AppInfo _appInfo;
    private readonly AppPaths _paths;
    private readonly JsonFileStore _store;
    private readonly AzureDevOpsClient _client;
    private readonly ReferenceParser _referenceParser;
    private readonly MarkdownExportService _markdownExportService;
    private readonly ConfigService _configService;

    public Application(
        AppInfo appInfo,
        AppPaths paths,
        JsonFileStore store,
        AzureDevOpsClient client,
        ReferenceParser referenceParser,
        MarkdownExportService markdownExportService)
    {
        _appInfo = appInfo;
        _paths = paths;
        _store = store;
        _client = client;
        _referenceParser = referenceParser;
        _markdownExportService = markdownExportService;
        _configService = new ConfigService(paths, store);
    }

    public async Task<int> RunAsync(string[] args)
    {
        try
        {
            SplashRenderer.Render();

            if (args.Length == 0)
            {
                var config = await _configService.LoadOrCreateAsync();
                await EnsureInteractiveStartupConfigAsync(config);
                await RunInteractiveAsync(config);
                return 0;
            }

            return await RunDirectCommandAsync(args);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
            return 1;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {SplashRenderer.EscapeMarkup(ex.Message)}");
            return 1;
        }
    }

    private async Task<int> RunDirectCommandAsync(string[] args)
    {
        if (args.Contains("--help", StringComparer.OrdinalIgnoreCase))
        {
            ShowHelp();
            return 0;
        }

        if (args.Contains("--version", StringComparer.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine($"[green]{_appInfo.Name}[/] version [blue]{_appInfo.Version}[/]");
            return 0;
        }

        var config = await _configService.LoadOrCreateAsync();

        return args[0] switch
        {
            "--set-pat" => await HandleSetPatAsync(args, config),
            "--set-org" => await HandleSetOrgAsync(args, config),
            "--set-project" => await HandleSetProjectAsync(args, config),
            "--get" => await HandleGetAsync(args, config),
            _ => throw new InvalidOperationException($"Unknown command '{args[0]}'. Run --help to see the supported commands.")
        };
    }

    private async Task RunInteractiveAsync(AppConfig config)
    {
        while (true)
        {
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an action")
                    .AddChoices(
                        "Retrieve Work Item",
                        "Configure Application",
                        "View Last Export Result",
                        "Help",
                        "Exit"));

            switch (selection)
            {
                case "Retrieve Work Item":
                    await EnsureRetrievalConfigAsync(config, allowPromptForPat: true);
                    await RetrieveAndExportInteractiveAsync(config);
                    break;
                case "Configure Application":
                    await RunConfigurationMenuAsync(config);
                    break;
                case "View Last Export Result":
                    ShowLastExport(config);
                    break;
                case "Help":
                    ShowHelp();
                    break;
                case "Exit":
                    Console.Clear();
                    return;
            }
        }
    }

    private async Task RunConfigurationMenuAsync(AppConfig config)
    {
        while (true)
        {
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Configuration")
                    .AddChoices(
                        "Set PAT",
                        "Set Organization",
                        "Set Project",
                        "Set Output Path",
                        "Return to Main Menu"));

            switch (selection)
            {
                case "Set PAT":
                    config.Pat = PromptRequiredText("Enter Azure DevOps PAT");
                    await _configService.SaveAsync(config);
                    break;
                case "Set Organization":
                    config.DefaultOrganization = PromptRequiredText("Enter Azure DevOps organization");
                    config.DefaultProjectId = null;
                    config.DefaultProjectName = null;
                    await _configService.SaveAsync(config);
                    break;
                case "Set Project":
                    await EnsureOrganizationAsync(config);
                    await EnsurePatAsync(config);
                    await SelectProjectAsync(config);
                    break;
                case "Set Output Path":
                    config.OutputRootPath = PromptPath("Enter the output path for markdown exports");
                    await _configService.SaveAsync(config);
                    break;
                case "Return to Main Menu":
                    return;
            }
        }
    }

    private async Task RetrieveAndExportInteractiveAsync(AppConfig config)
    {
        var workItemId = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter the work item ID")
                .Validate(id => id > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Work item ID must be positive.[/]")));

        var outputOverride = AnsiConsole.Confirm("Override the default output path for this export?", false)
            ? PromptPath("Enter a one-off output path")
            : null;

        var exportPath = await RetrieveAndExportAsync(config, workItemId, outputOverride);
        AnsiConsole.MarkupLine($"[green]Export complete:[/] {SplashRenderer.EscapeMarkup(exportPath)}");
    }

    private async Task<int> HandleSetPatAsync(string[] args, AppConfig config)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("Usage: ado-wi --set-pat <PAT>");
        }

        config.Pat = args[1];
        await _configService.SaveAsync(config);
        AnsiConsole.MarkupLine("[green]PAT updated.[/]");
        return 0;
    }

    private async Task<int> HandleSetOrgAsync(string[] args, AppConfig config)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("Usage: ado-wi --set-org <OrgName>");
        }

        config.DefaultOrganization = args[1];
        config.DefaultProjectId = null;
        config.DefaultProjectName = null;
        await _configService.SaveAsync(config);
        AnsiConsole.MarkupLine($"[green]Default organization set to[/] {SplashRenderer.EscapeMarkup(args[1])}");
        return 0;
    }

    private async Task<int> HandleSetProjectAsync(string[] args, AppConfig config)
    {
        await EnsureOrganizationAsync(config);
        await EnsurePatAsync(config);

        if (args.Length == 1)
        {
            await SelectProjectAsync(config);
            AnsiConsole.MarkupLine($"[green]Default project set to[/] {SplashRenderer.EscapeMarkup(config.DefaultProjectName ?? string.Empty)}");
            return 0;
        }

        var input = args[1];
        var projects = await LoadProjectsAsync(config);
        var match = projects.FirstOrDefault(project =>
            string.Equals(project.Id, input, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(project.Name, input, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            throw new InvalidOperationException($"No visible project matched '{input}'.");
        }

        config.DefaultProjectId = match.Id;
        config.DefaultProjectName = match.Name;
        await _configService.SaveAsync(config);
        AnsiConsole.MarkupLine($"[green]Default project set to[/] {SplashRenderer.EscapeMarkup(match.Name)} ({match.Id})");
        return 0;
    }

    private async Task<int> HandleGetAsync(string[] args, AppConfig config)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out var workItemId) || workItemId <= 0)
        {
            throw new InvalidOperationException("Usage: ado-wi --get <WorkItemId> [--out <OutputPath>]");
        }

        string? outputOverride = null;
        if (args.Length > 2)
        {
            if (args.Length != 4 || !string.Equals(args[2], "--out", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Usage: ado-wi --get <WorkItemId> [--out <OutputPath>]");
            }

            outputOverride = args[3];
        }

        await EnsureRetrievalConfigAsync(config, allowPromptForPat: false);
        var exportPath = await RetrieveAndExportAsync(config, workItemId, outputOverride);
        AnsiConsole.MarkupLine($"[green]Export complete:[/] {SplashRenderer.EscapeMarkup(exportPath)}");
        return 0;
    }

    private async Task<string> RetrieveAndExportAsync(AppConfig config, int workItemId, string? outputOverride)
    {
        var retrievalService = new WorkItemRetrievalService(config, _client, _referenceParser);

        var result = await AnsiConsole.Status()
            .StartAsync("Retrieving work item data...", async _ =>
                await retrievalService.RetrieveAsync(workItemId));

        var outputRoot = string.IsNullOrWhiteSpace(outputOverride) ? config.OutputRootPath : outputOverride!;
        var exportPath = _paths.GetExportPath(outputRoot, config.DefaultProjectName!, workItemId, result.RootWorkItem.Title);
        var markdown = _markdownExportService.BuildMarkdown(result);
        await _store.WriteTextAsync(exportPath, markdown);

        config.LastExportPath = exportPath;
        await _configService.SaveAsync(config);
        return exportPath;
    }

    private async Task EnsureInteractiveStartupConfigAsync(AppConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.OutputRootPath))
        {
            config.OutputRootPath = PromptPath("First run setup: enter the output path for markdown exports");
            await _configService.SaveAsync(config);
        }

        if (string.IsNullOrWhiteSpace(config.DefaultOrganization))
        {
            config.DefaultOrganization = PromptRequiredText("First run setup: enter the default Azure DevOps organization");
            config.DefaultProjectId = null;
            config.DefaultProjectName = null;
            await _configService.SaveAsync(config);
        }

        if (string.IsNullOrWhiteSpace(config.DefaultProjectId) || string.IsNullOrWhiteSpace(config.DefaultProjectName))
        {
            if (string.IsNullOrWhiteSpace(config.Pat))
            {
                config.Pat = PromptRequiredText("First run setup: enter the Azure DevOps PAT needed to list projects");
                await _configService.SaveAsync(config);
            }

            await SelectProjectAsync(config);
        }
    }

    private async Task EnsureRetrievalConfigAsync(AppConfig config, bool allowPromptForPat)
    {
        if (string.IsNullOrWhiteSpace(config.OutputRootPath))
        {
            throw new InvalidOperationException("No output path is configured. Set one in interactive mode or configure it before running --get.");
        }

        if (string.IsNullOrWhiteSpace(config.DefaultOrganization))
        {
            throw new InvalidOperationException("No default organization is configured.");
        }

        if (string.IsNullOrWhiteSpace(config.Pat))
        {
            if (!allowPromptForPat)
            {
                throw new InvalidOperationException("No PAT is configured. Use --set-pat first.");
            }

            config.Pat = PromptRequiredText("Enter the Azure DevOps PAT");
            await _configService.SaveAsync(config);
        }

        if (string.IsNullOrWhiteSpace(config.DefaultProjectId) || string.IsNullOrWhiteSpace(config.DefaultProjectName))
        {
            await SelectProjectAsync(config);
        }
    }

    private async Task EnsureOrganizationAsync(AppConfig config)
    {
        if (!string.IsNullOrWhiteSpace(config.DefaultOrganization))
        {
            return;
        }

        config.DefaultOrganization = PromptRequiredText("Enter Azure DevOps organization");
        await _configService.SaveAsync(config);
    }

    private async Task EnsurePatAsync(AppConfig config)
    {
        if (!string.IsNullOrWhiteSpace(config.Pat))
        {
            return;
        }

        config.Pat = PromptRequiredText("Enter Azure DevOps PAT");
        await _configService.SaveAsync(config);
    }

    private async Task SelectProjectAsync(AppConfig config)
    {
        var projects = await AnsiConsole.Status()
            .StartAsync("Loading visible projects...", async _ => await LoadProjectsAsync(config));

        if (projects.Count == 0)
        {
            throw new InvalidOperationException("No visible projects were returned for the configured organization.");
        }

        var selectedProject = AnsiConsole.Prompt(
            new SelectionPrompt<AdoProject>()
                .Title("Select the default project")
                .UseConverter(project => project.ToString())
                .AddChoices(projects.OrderBy(project => project.Name)));

        config.DefaultProjectId = selectedProject.Id;
        config.DefaultProjectName = selectedProject.Name;
        await _configService.SaveAsync(config);
    }

    private async Task<IReadOnlyList<AdoProject>> LoadProjectsAsync(AppConfig config)
    {
        return await _client.ListProjectsAsync(
            config.AzureDevOpsBaseUrl,
            config.DefaultOrganization!,
            config.Pat!);
    }

    private void ShowLastExport(AppConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.LastExportPath))
        {
            AnsiConsole.MarkupLine("[yellow]No export has been generated yet.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Last export:[/] {SplashRenderer.EscapeMarkup(config.LastExportPath)}");
    }

    private void ShowHelp()
    {
        var rows = new[]
        {
            ("ado-wi", "Launch the interactive menu system."),
            ("ado-wi --help", "Show the splash, command list, command option behavior, and interactive mode notes."),
            ("ado-wi --version", "Show the splash and current application version."),
            ("ado-wi --set-pat <PAT>", "Store the Azure DevOps PAT in ~/.ADO-WI/ADO-WI.config."),
            ("ado-wi --set-org <OrgName>", "Store the default Azure DevOps organization and clear the current project selection."),
            ("ado-wi --set-project", "Load visible projects and let you pick the default project."),
            ("ado-wi --set-project <ProjectId || ProjectName>", "Resolve and store the default project as both project ID and project name."),
            ("ado-wi --get <WorkItemId>", "Retrieve the configured work item hierarchy and export markdown to the default output path."),
            ("ado-wi --get <WorkItemId> --out <OutputPath>", "Run the same retrieval but write the markdown export to a one-off output path.")
        };

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Command");
        table.AddColumn("Behavior");

        foreach (var (command, behavior) in rows)
        {
            table.AddRow(SplashRenderer.EscapeMarkup(command), SplashRenderer.EscapeMarkup(behavior));
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Interactive mode[/]");
        AnsiConsole.MarkupLine("Running [blue]ado-wi[/] without a direct command opens the menu system.");
        AnsiConsole.MarkupLine("The menu supports retrieving work items, configuring the app, viewing the last export path, and opening help.");
    }

    private static string PromptRequiredText(string prompt)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .Validate(value => string.IsNullOrWhiteSpace(value)
                    ? ValidationResult.Error("[red]A value is required.[/]")
                    : ValidationResult.Success()))
            .Trim();
    }

    private static string PromptPath(string prompt)
    {
        var path = AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .Validate(value => string.IsNullOrWhiteSpace(value)
                    ? ValidationResult.Error("[red]A path is required.[/]")
                    : ValidationResult.Success()))
            .Trim();

        var expanded = ExpandHomePath(path);
        Directory.CreateDirectory(expanded);
        return expanded;
    }

    private static string ExpandHomePath(string path)
    {
        if (!path.StartsWith("~/", StringComparison.Ordinal))
        {
            return Path.GetFullPath(path);
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, path[2..]);
    }
}
