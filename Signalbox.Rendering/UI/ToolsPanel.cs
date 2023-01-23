using Signalbox.Engine.MainManager;
using Signalbox.Engine.Tools;
using Signalbox.Engine.Utilities;
using Signalbox.Rendering.UIFramework;

namespace Signalbox.Rendering.UI;

[Order(100)]
public class ToolsPanel : ButtonPanelBase
{
    private readonly ISignalboxManager _signalboxManager;
    private readonly ButtonBase _switchModeButton;
    private readonly List<ButtonBase> _buildModeButtons;
    private readonly List<ButtonBase> _playModeButtons;

    protected override int Top => -12;

    protected override bool IsCollapsable => false;
    protected override string? Title => "Mode";

    public ToolsPanel(IEnumerable<ITool> tools, ISignalboxManager signalboxManager)
    {
        _signalboxManager = signalboxManager;

        _signalboxManager.Changed += (s, e) => OnChanged();

        _switchModeButton = new BuildModeButton(_signalboxManager);

        _buildModeButtons = tools.Where(t => ShouldShowTool(true, t)).Select(tool => new TextButton(tool.Name, () => tool == _signalboxManager.CurrentTool, () => _signalboxManager.CurrentTool = tool)).ToList<ButtonBase>();
        _playModeButtons = tools.Where(t => ShouldShowTool(false, t)).Select(tool => new TextButton(tool.Name, () => tool == _signalboxManager.CurrentTool, () => _signalboxManager.CurrentTool = tool)).ToList<ButtonBase>();

        _buildModeButtons.Insert(0, _switchModeButton);
        _playModeButtons.Insert(0, _switchModeButton);
    }

    protected override IEnumerable<ButtonBase> GetButtons()
        => _signalboxManager.BuildMode ? _buildModeButtons : _playModeButtons;

    private static bool ShouldShowTool(bool buildMode, ITool tool)
        => (buildMode, tool.Mode) switch
        {
            (true, ToolMode.Build) => true,
            (false, ToolMode.Play) => true,
            (_, ToolMode.All) => true,
            _ => false
        };
}
