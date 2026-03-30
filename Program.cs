using WorkItems.ConsoleUi;
using WorkItems.Services;
using WorkItems.Workflows;

var app = new Application(
    new AppInfo("ado-wi", "0.1.0"),
    new AppPaths(),
    new JsonFileStore(),
    new AzureDevOpsClient(),
    new ReferenceParser(),
    new MarkdownExportService());

return await app.RunAsync(args);
