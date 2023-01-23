using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.UIFramework;

public interface IScreenManager
{
    /// <summary>
    /// Called when the actual screen is being repainted
    /// </summary>
    void Render(ICanvas canvas);
}
