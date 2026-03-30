using WorkItems.Models;

namespace WorkItems.Services;

public sealed class ConfigService
{
    private readonly AppPaths _paths;
    private readonly JsonFileStore _store;

    public ConfigService(AppPaths paths, JsonFileStore store)
    {
        _paths = paths;
        _store = store;
    }

    public async Task<AppConfig> LoadOrCreateAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_paths.GetStorageRoot());

        var config = await _store.LoadAsync<AppConfig>(_paths.GetConfigPath(), cancellationToken);
        if (config is not null)
        {
            if (string.IsNullOrWhiteSpace(config.StorageRootPath))
            {
                config.StorageRootPath = _paths.GetStorageRoot();
            }

            return config;
        }

        var created = new AppConfig
        {
            StorageRootPath = _paths.GetStorageRoot()
        };

        await SaveAsync(created, cancellationToken);
        return created;
    }

    public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        config.StorageRootPath = _paths.GetStorageRoot();
        config.UpdatedUtc = DateTime.UtcNow;
        return _store.SaveAsync(_paths.GetConfigPath(), config, cancellationToken);
    }
}
