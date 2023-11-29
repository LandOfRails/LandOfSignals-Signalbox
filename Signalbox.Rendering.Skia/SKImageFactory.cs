using Signalbox.Rendering.Drawing;
using SkiaSharp;

namespace Signalbox.Rendering.Skia;

public class SKImageFactory : IImageFactory
{
    private GRContext? _context;

    public IImageCanvas CreateImageCanvas(int width, int height)
    {
        return new SKSurfaceWrapper(width, height, _context);
    }

    public bool SetContext(IContext context)
    {
        if (context is SKContextWrapper skContext)
        {
            var initalSet = _context == null;
            _context = skContext.Context;
            return initalSet;
        }
        return false;
    }
}
