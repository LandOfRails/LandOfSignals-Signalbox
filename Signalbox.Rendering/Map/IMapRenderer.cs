using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.Map;

public interface IMapRenderer
{
    event EventHandler? Changed;

    IImage GetMapImage();
}
