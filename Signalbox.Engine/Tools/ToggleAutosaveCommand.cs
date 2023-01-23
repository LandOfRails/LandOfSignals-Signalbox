using Signalbox.Engine.StateManager;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tools;

[Order(140)]
public class ToggleAutosaveCommand : ICommand
{
    private readonly ISignalboxStateManager _signalboxStateManager;

    public string Name => "Toggle Autosave";

    public ToggleAutosaveCommand(ISignalboxStateManager signalboxStateManager)
    {
        _signalboxStateManager = signalboxStateManager;
    }

    public void Execute()
    {
        _signalboxStateManager.AutosaveEnabled = !_signalboxStateManager.AutosaveEnabled;
    }
}
