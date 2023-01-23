namespace Signalbox.Rendering.UIFramework;

public interface IInteractionHandler
{
    bool PreHandleNextClick { get; }

    bool HandlePointerAction(int x, int y, int width, int height, PointerAction action);
}
