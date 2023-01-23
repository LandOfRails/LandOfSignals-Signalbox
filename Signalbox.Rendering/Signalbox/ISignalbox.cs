using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.Signalbox;

public interface ISignalbox : IDisposable
{
    void AdjustViewPortIfNecessary();
    void Render(ICanvas canvas);
    void SetSize(int width, int height);
    (int Width, int Height) GetSize();
    (int Width, int Height) GetScreenSize();
    void SetContext(IContext context);

    Task InitializeAsync(int columns, int rows);
}
