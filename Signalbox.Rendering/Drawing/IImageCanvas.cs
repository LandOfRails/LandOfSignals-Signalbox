namespace Signalbox.Rendering.Drawing;

public interface IImageCanvas : IDisposable
{
    ICanvas Canvas { get; }

    IImage Render();
}
