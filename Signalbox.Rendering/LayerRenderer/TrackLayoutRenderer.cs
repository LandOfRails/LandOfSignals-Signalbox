using Signalbox.Engine.Entity;
using Signalbox.Engine.Tracks;
using Signalbox.Engine.Utilities;
using Signalbox.Rendering.Drawing;
using Signalbox.Rendering.StaticEntityRenderer;

namespace Signalbox.Rendering.LayerRenderer;

[Order(450)]
public class TrackLayoutRenderer : StaticEntityCollectionRenderer<Track>
{
    public TrackLayoutRenderer(ILayout<Track> layout, IEnumerable<IStaticEntityRenderer<Track>> renderers, IImageFactory imageFactory, IImageCache imageCache)
            : base(layout, renderers, imageFactory, imageCache)
    {
        this.Name = "Tracks";
    }
}
