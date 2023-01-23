using Signalbox.Engine.StateManager;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tools;

[Order(10)]
public class ClearAllCommand : ICommand
{
    private readonly ISignalboxStateManager _signalboxStateManager;

    public string Name => "Clear All";

    public ClearAllCommand(ISignalboxStateManager signalboxStateManager)
    {
        _signalboxStateManager = signalboxStateManager;
    }

    public void Execute()
    {
        _signalboxStateManager.Reset();
    }
}
