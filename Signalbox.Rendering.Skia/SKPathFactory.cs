using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.Skia;

public class SKPathFactory : IPathFactory
{
    public IPath Create()
    {
        return new SKPathWrapper();
    }
}
