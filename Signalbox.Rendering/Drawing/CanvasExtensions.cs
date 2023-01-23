namespace Signalbox.Rendering.Drawing;

public static partial class CanvasExtensions
{
    public static IDisposable Scope(this ICanvas canvas)
    {
        return new CanvasScope(canvas);
    }
}
