using Blazored.LocalStorage;
using Signalbox.Engine.Storage;

namespace LandOfSignals_Signalbox;

public class BlazorSignalboxStorage : ISignalboxStorage
{
    private ISyncLocalStorageService? _syncLocalStorageService;
    private readonly Dictionary<string, string> _lastSavedValue = new();

    public IServiceProvider? AspNetCoreServices { get; set; }

    private ISyncLocalStorageService? SyncLocalStorageService => (_syncLocalStorageService ??= this.AspNetCoreServices?.GetService<ISyncLocalStorageService>());

    public string? Read(string key)
    {
        var data = this.SyncLocalStorageService?.GetItemAsString(key);
        return data;
    }

    public void Write(string key, string value)
    {
        var valueExists = _lastSavedValue.TryGetValue(key, out var previousValue);
        if (!valueExists || previousValue != value)
        {
            _lastSavedValue[key] = value;
            this.SyncLocalStorageService?.SetItemAsString(key, value);
        }
    }
}
