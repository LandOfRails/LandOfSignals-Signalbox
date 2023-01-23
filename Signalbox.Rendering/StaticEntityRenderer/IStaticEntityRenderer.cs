using Signalbox.Engine.Entity;
using Signalbox.Rendering.LayerRenderer.Bases;

namespace Signalbox.Rendering.StaticEntityRenderer;

public interface IStaticEntityRenderer<T> : IRenderer<T> where T : IStaticEntity
{
}
