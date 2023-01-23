using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.UIFramework;

public interface IScreen
{
    event EventHandler? Changed;

    void Render(ICanvas canvas, int width, int height);
}
