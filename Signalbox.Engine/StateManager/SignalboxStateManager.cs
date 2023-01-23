﻿using Signalbox.Engine.MainManager;
using Signalbox.Engine.Storage;
using Signalbox.Engine.Utilities;
using Signalbox.Instrumentation;
using Signalbox.Instrumentation.Stats;

namespace Signalbox.Engine.StateManager;

[Order(999999)]
public class SignalboxStateManager : ISignalboxStateManager, IInitializeAsync, ISignalboxStep
{
    // Auto-save every 2 seconds
    private const int AutosaveInterval = 2 * 60;

    private int _autosaveCounter;
    private bool _autosaveEnabled;

    private readonly IEnumerable<ISignalboxState> _gameStates;
    private readonly ISignalboxStorage _storage;
    private readonly InformationStat _autosaveStat = InstrumentationBag.Add<InformationStat>("Autosave");
    private readonly ElapsedMillisecondsTimedStat _saveTimeStat = InstrumentationBag.Add<ElapsedMillisecondsTimedStat>("Save-Time");

    public bool AutosaveEnabled
    {
        get => _autosaveEnabled;
        set
        {
            _autosaveEnabled = value;
            if (_autosaveEnabled)
            {
                _autosaveCounter = 0;
                Save();
            }
            else
            {
                // if they're turning off autosave, we want to at least save that
                // otherwise if they leave before the next save, it could be back on
                // on load
                _storage.Write("Autosave", this.AutosaveEnabled.ToString());
            }
        }
    }

    public SignalboxStateManager(IEnumerable<ISignalboxState> gameStates, ISignalboxStorage storage)
    {
        _gameStates = gameStates;
        _storage = storage;
    }

    public Task InitializeAsync(int columns, int rows)
    {
        Load();

        return Task.CompletedTask;
    }

    public void Load()
    {
        foreach (var gameState in _gameStates)
        {
            if (!gameState.Load(_storage))
            {
                // If any one failed, reset the whole game because who knows what might happen
                Reset();
                break;
            }
        }
        this.AutosaveEnabled = _storage.Read("Autosave")?.Equals("True") ?? true;
    }

    public void Reset()
    {
        foreach (var gameState in _gameStates)
        {
            gameState.Reset();
        }
    }

    public void Save()
    {
        using (_saveTimeStat.Measure())
        {
            foreach (var gameState in _gameStates)
            {
                gameState.Save(_storage);
            }
            _storage.Write("Autosave", this.AutosaveEnabled.ToString());
        }
    }

    public void Update(long timeSinceLastTick)
    {
        _autosaveStat.Information = this.AutosaveEnabled ? "On" : "Off";

        if (this.AutosaveEnabled &&
            ++_autosaveCounter > AutosaveInterval)
        {
            Save();
            _autosaveCounter = 0;
        }
    }
}
