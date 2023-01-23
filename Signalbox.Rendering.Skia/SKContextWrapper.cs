using Signalbox.Rendering.Drawing;
using SkiaSharp;

namespace Signalbox.Rendering.Skia;

public class SKContextWrapper : IContext
{
    public GRContext Context { get; }

    public SKContextWrapper(GRContext context)
    {
        this.Context = context;
    }
}
