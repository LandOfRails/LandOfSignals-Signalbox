using Signalbox.Engine.StateManager;
using Signalbox.Engine.Utilities;

namespace Signalbox.Engine.Tools;

[Order(150)]
public class SaveCommand : ICommand
{
    private readonly ISignalboxStateManager _signalboxStateManager;

    public string Name => "Save";

    public SaveCommand(ISignalboxStateManager signalboxStateManager)
    {
        _signalboxStateManager = signalboxStateManager;
    }

    public void Execute()
    {
        _signalboxStateManager.Save();
    }
}
