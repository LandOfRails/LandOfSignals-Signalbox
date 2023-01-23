using Signalbox.Engine.Storage;

namespace Signalbox.Engine.StateManager;

public interface ISignalboxState
{
    bool Load(ISignalboxStorage storage);
    void Save(ISignalboxStorage storage);
    void Reset();
}
