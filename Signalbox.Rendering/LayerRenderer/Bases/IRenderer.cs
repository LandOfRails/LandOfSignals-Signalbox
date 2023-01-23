using Signalbox.Rendering.Drawing;

namespace Signalbox.Rendering.LayerRenderer.Bases;

public interface IRenderer<T>
{
    void Render(ICanvas canvas, T entity);
    bool ShouldRender(T entity) => true;
}
