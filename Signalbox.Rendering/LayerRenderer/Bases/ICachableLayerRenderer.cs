namespace Signalbox.Rendering.LayerRenderer.Bases;

public interface ICachableLayerRenderer : ILayerRenderer
{
    event EventHandler? Changed;
}
