using Signalbox.Engine.MainManager;
using Signalbox.Rendering.UIFramework;

namespace Signalbox.Rendering.UI;

public class BuildModeButton : MultiButton
{
    public BuildModeButton(global::Signalbox.Engine.MainManager.ISignalboxManager signalboxManager)
        : base(34, GetButtons(signalboxManager))
    {
    }

    private static ButtonBase[] GetButtons(ISignalboxManager signalboxManager)
    {
        return new ButtonBase[]{
                new PictureButton(Picture.Tools, 20, () => signalboxManager.BuildMode, () => signalboxManager.BuildMode = true),
                new PictureButton(Picture.Play, 20, () => !signalboxManager.BuildMode, () => signalboxManager.BuildMode = false)
                };
    }
}
