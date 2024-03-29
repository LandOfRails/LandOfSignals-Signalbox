﻿using Blazored.LocalStorage;
using Signalbox.Engine.Storage;

namespace LandOfSignals_Signalbox.Client;

public class BlazorGameStorage : ISignalboxStorage
{
    private ISyncLocalStorageService? _syncLocalStorageService;
    private readonly Dictionary<string, string> _lastSavedValue = new Dictionary<string, string>();

    public IServiceProvider? AspNetCoreServices { get; set; }

    private ISyncLocalStorageService? SyncLocalStorageService => (_syncLocalStorageService ??= AspNetCoreServices?.GetService<ISyncLocalStorageService>());

    public string? Read(string key)
    {
        var data = SyncLocalStorageService?.GetItemAsString(key);
        return data;
    }

    public void Write(string key, string value)
    {
        var valueExists = _lastSavedValue.TryGetValue(key, out var previousValue);
        if (!valueExists || previousValue != value)
        {
            _lastSavedValue[key] = value;
            SyncLocalStorageService?.SetItemAsString(key, value);
        }
    }
}
