using Signalbox.Engine.Tools;

namespace Signalbox.Engine.MainManager;

public interface ISignalboxManager : IDisposable
{
    event EventHandler? Changed;

    ITool CurrentTool { get; set; }
    bool BuildMode { get; set; }
}
