using Signalbox.Engine.Utilities;
using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.LayerRenderer.Bases;

public interface ILayerRenderer : ITogglable
{
    void Render(ICanvas canvas, int width, int height, IPixelMapper pixelMapper);
}
