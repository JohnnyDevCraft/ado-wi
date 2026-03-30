using WorkItems.ConsoleUi;
using WorkItems.Services;
using WorkItems.Workflows;

var assemblyVersion =
    typeof(Program).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
        .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
        .FirstOrDefault()?.InformationalVersion
    ?? typeof(Program).Assembly.GetName().Version?.ToString()
    ?? "0.0.0";

var app = new Application(
    new AppInfo("ado-wi", assemblyVersion),
    new AppPaths(),
    new JsonFileStore(),
    new AzureDevOpsClient(),
    new ReferenceParser(),
    new MarkdownExportService());

return await app.RunAsync(args);
