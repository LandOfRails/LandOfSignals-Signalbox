namespace Signalbox.Engine.StateManager;

public interface ISignalboxStateManager
{
    bool AutosaveEnabled { get; set; }

    void Load();
    void Save();
    void Reset();
}
