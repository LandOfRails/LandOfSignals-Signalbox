namespace Signalbox.Rendering.Drawing;

public interface IImageFactory
{
    IImageCanvas CreateImageCanvas(int width, int height);
    bool SetContext(IContext context);
}
